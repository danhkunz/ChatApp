using ChatApp.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ChatContext>(option => option.UseSqlite("Data source=ChatApp.db"));
AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


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
    // string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{TraceId}] [{UserId}] {Message:lj}{NewLine}{Exception}";

    // Log.Logger = new LoggerConfiguration()
    //                 .MinimumLevel.Debug()
    //                 .Enrich.FromLogContext()
    //                 .WriteTo.Async(a =>
    //                 {
    //                     a.Console(outputTemplate: template);
    //                 })
    //                 .WriteTo.Logger(lc =>
    //                 {
    //                     lc.WriteTo.Map("UserId", string.Empty, (name, wt) => wt.Async(a =>
    //                     {
    //                         a.File($"{Const.LOGS}/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{DateTime.Now:dd}/{name}/.log",
    //                                 outputTemplate: template,
    //                                 rollingInterval: RollingInterval.Day,
    //                                 fileSizeLimitBytes: 10485760,
    //                                 rollOnFileSizeLimit: true,
    //                                 shared: true);
    //                     }));
    //                 })
    //                 .CreateLogger();
}

void addAuthenticate()
{ }
