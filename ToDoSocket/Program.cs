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
        var connectionManager = context.RequestServices.GetService<WebSocketConnectionManager>();
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var connectionId = Guid.NewGuid().ToString();

        await connectionManager.AddConnectionAsync(connectionId, webSocket);
        await HandleWebSocketAsync(connectionId, webSocket, connectionManager);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});


app.Run();

static async Task HandleWebSocketAsync(string connectionId, WebSocket webSocket, WebSocketConnectionManager connectionManager)
{
    var buffer = new byte[1024 * 4];

    try
    {
        // Send welcome message
        var welcomeMessage = new { type = "welcome", message = $"Connection {connectionId} established!" };
        await SendMessageAsync(webSocket, JsonSerializer.Serialize(welcomeMessage));

        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

                var parsedMessage = JsonDocument.Parse(messageJson).RootElement;

                await connectionManager.BroadcastAsync(JsonSerializer.Serialize(parsedMessage), connectionId);
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

    public async Task AddConnectionAsync(string connectionId, WebSocket webSocket)
    {
        _connections.TryAdd(connectionId, webSocket);
        Console.WriteLine($"Connection added: {connectionId}. Total connections: {_connections.Count}");

        // Notify all clients about new connection
        var notification = new { type = "user_joined", connectionId = connectionId, totalConnections = _connections.Count };
        await BroadcastAsync(JsonSerializer.Serialize(notification), connectionId);
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        if (_connections.TryRemove(connectionId, out var webSocket))
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }

            Console.WriteLine($"Connection removed: {connectionId}. Total connections: {_connections.Count}");

            // Notify remaining clients
            var notification = new { type = "user_left", connectionId = connectionId, totalConnections = _connections.Count };
            await BroadcastAsync(JsonSerializer.Serialize(notification));
        }
    }

    public async Task BroadcastAsync(string message, string excludeConnectionId = null)
    {
        var tasks = new List<Task>();

        foreach (var connection in _connections)
        {
            if (connection.Key != excludeConnectionId && connection.Value.State == WebSocketState.Open)
            {
                tasks.Add(SendMessageToConnectionAsync(connection.Value, message));
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
