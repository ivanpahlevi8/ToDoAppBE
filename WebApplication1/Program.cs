using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    // Hardcode the production server right into the generator
    c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
    {
        Url = "https://ivan-portofolio.xyz/todoapp",
        Description = "Production Server"
    });
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

// add identity manager
builder.Services.AddIdentity<UserModel, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// add dependency for mapper configuration
IMapper mapper = Mappingconfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// service dependency section
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ITeamService, TeamService>();

builder.Services.AddScoped<IConnectionService, ConnectionService>();

builder.Services.AddScoped<ITodoService, ToDoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UsePathBase("/todoapp");

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "v1");
    // Optional: To serve Swagger UI at the app's root (http://localhost:5000/)
    // options.RoutePrefix = string.Empty; 
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
