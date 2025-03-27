using ChatApp.ChatHubs;
using ChatApp.DBContext;
using ChatApp.Helpers;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using ChatApp.Caches;
using ChatApp.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration
                            .AddJsonFile(builder.Configuration["SettingFile"].ToString(),
                                        false,
                                        reloadOnChange: true)
                            .Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ChatContext>(option => option.UseSqlite("Data source=ChatApp.db"));
AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<Cache>(provider => new Cache("AppCatch"));
builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting(); 
app.MapHub<ChatHub>("/chatHub");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers(); 

app.UseHttpsRedirection();


app.Run();
void AddSwaggerGen()
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ChatApp API",
            Version = "v1"
        });
    });
}

void addSerilog()
{
    string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{TraceId}] [{UserId}] {Message:lj}{NewLine}{Exception}";

    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .WriteTo.Async(a =>
                    {
                        a.Console(outputTemplate: template);
                    })
                    .WriteTo.Logger(lc =>
                    {
                        lc.WriteTo.Map("UserId", string.Empty, (name, wt) => wt.Async(a =>
                        {
                            a.File($"{Const.LOGS}/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{DateTime.Now:dd}/{name}/.log",
                                    outputTemplate: template,
                                    rollingInterval: RollingInterval.Day,
                                    fileSizeLimitBytes: 10485760,
                                    rollOnFileSizeLimit: true,
                                    shared: true);
                        }));
                    })
                    .CreateLogger();
}

void AddAuthentication()
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false
            };

            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies[Const.ACCESS_TOKEN];
                    if (string.IsNullOrWhiteSpace(token) == false)
                    {
                        var cache = context.Request.HttpContext.RequestServices.GetKeyedService<Cache>(Const.JWT_CACHE);
                        if (cache.GetValue(token, out string shaincd))
                        {
                            context.Token = token;

                            using var scope = context.Request.HttpContext.RequestServices.CreateScope();
                            var configs = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<Configs>>().Value;
                            cache.UpdateAsync(token, shaincd, configs.ExpireToken).GetAwaiter().GetResult();
                        }
                    }
                    return Task.CompletedTask;
                },
            };
        });
}
