using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll");
app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        // Extract projectId from the query string sent by Android
        string room = context.Request.Query["projectId"].FirstOrDefault() ?? "default";

        var connectionManager = context.RequestServices.GetService<WebSocketConnectionManager>();
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var connectionId = Guid.NewGuid().ToString();

        // Pass the room down
        await connectionManager.AddConnectionAsync(connectionId, webSocket, room);
        await HandleWebSocketAsync(connectionId, webSocket, connectionManager, room);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});


app.Run();

static async Task HandleWebSocketAsync(string connectionId, WebSocket webSocket, WebSocketConnectionManager connectionManager, string roomId)
{
    var buffer = new byte[1024 * 4];

    try
    {
        var welcomeMessage = new { type = "welcome", message = $"Connection {connectionId} established in room {roomId}!" };
        await SendMessageAsync(webSocket, JsonSerializer.Serialize(welcomeMessage));

        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var parsedMessage = JsonDocument.Parse(messageJson).RootElement;

                // Broadcast ONLY to this specific room
                await connectionManager.BroadcastToRoomAsync(roomId, JsonSerializer.Serialize(parsedMessage), connectionId);
            }

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await connectionManager.RemoveConnectionAsync(connectionId);
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error handling WebSocket connection {connectionId}: {ex.Message}");
        await connectionManager.RemoveConnectionAsync(connectionId);
    }
}

static async Task SendMessageAsync(WebSocket webSocket, string message)
{
    if (webSocket.State == WebSocketState.Open)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

// Connection Manager Class
public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _connections = new();
    // Add this to track which room a connection belongs to
    private readonly ConcurrentDictionary<string, string> _connectionRooms = new();

    public async Task AddConnectionAsync(string connectionId, WebSocket webSocket, string roomId)
    {
        _connections.TryAdd(connectionId, webSocket);
        _connectionRooms.TryAdd(connectionId, roomId); // Save the room mapping

        Console.WriteLine($"Connection {connectionId} added to room {roomId}.");

        var notification = new { type = "user_joined", connectionId = connectionId, roomId = roomId };
        await BroadcastToRoomAsync(roomId, JsonSerializer.Serialize(notification), connectionId);
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        if (_connections.TryRemove(connectionId, out var webSocket))
        {
            // Remove from room tracking and get the room ID
            _connectionRooms.TryRemove(connectionId, out var roomId);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }

            Console.WriteLine($"Connection removed: {connectionId}.");

            // Notify remaining clients in that specific room
            if (!string.IsNullOrEmpty(roomId))
            {
                var notification = new { type = "user_left", connectionId = connectionId, roomId = roomId };
                await BroadcastToRoomAsync(roomId, JsonSerializer.Serialize(notification));
            }
        }
    }

    // Replace the old broadcast with this room-specific broadcast
    public async Task BroadcastToRoomAsync(string roomId, string message, string excludeConnectionId = null)
    {
        var tasks = new List<Task>();

        foreach (var connection in _connections)
        {
            var connId = connection.Key;
            var socket = connection.Value;

            // Check if the connection belongs to the target room
            if (_connectionRooms.TryGetValue(connId, out var connectionRoom) && connectionRoom == roomId)
            {
                if (connId != excludeConnectionId && socket.State == WebSocketState.Open)
                {
                    tasks.Add(SendMessageToConnectionAsync(socket, message));
                }
            }
        }

        await Task.WhenAll(tasks);
    }

    private async Task SendMessageToConnectionAsync(WebSocket webSocket, string message)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public int GetConnectionCount() => _connections.Count;
}
