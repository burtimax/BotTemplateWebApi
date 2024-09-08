using System.Reflection;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Models;
using MultipleBotFramework.Options;
using MultipleTestBot.App.Options;
using MultipleTestBot.Db.AppDb;
using MultipleTestBot.Endpoints.Bot.GetBots;
using MultipleTestBot.Extensions;
using MultipleTestBot.Resources;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwagger("Telegram Bot API");
builder.Services.AddFastEndpoints(o =>
    {
        o.Assemblies = new[]
        {
            typeof(GetBotsEndpoint).Assembly,
            typeof(Program).Assembly,
        };
    })
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Multiple Bots Template Project API";
            s.Version = "v1";
        };
    });


var services = builder.Services;
services.AddServices();

// Добавляем контексты
services.Configure<AppDbConnections>(builder.Configuration.GetSection(AppDbConnections.Section));
var dbConfigs = builder.Configuration.GetSection(AppDbConnections.Section).Get<AppDbConnections>();
services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(dbConfigs.AppDbConnetion,
            sqlOptions => sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
        options.EnableSensitiveDataLogging();
    }
);

// Регистрируем конфигурации.
services.Configure<BotConfiguration>(builder.Configuration.GetSection("Bot"));
services.Configure<BotOptions>(builder.Configuration.GetSection("BotOptions"));
var botConfig = builder.Configuration.GetSection("Bot").Get<BotConfiguration>();
BotOptions botOptions = builder.Configuration.GetSection("BotOptions").Get<BotOptions>();
BotResources botResources = services.ConfigureBotResources(botConfig.ResourcesFilePath);
services.AddBot(botConfig, botOptions: botOptions); // Подключаем бота
services.AddControllers();//.AddNewtonsoftJson(); //Обязательно подключаем NewtonsoftJson
services.AddHttpContextAccessor();
services.AddCors();
services.AddMapster();

// Свои сервисы


// Регистрируем контексты к базам данных.

var app = builder.Build();
// app.UseSwagger();
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
//     // options.RoutePrefix = string.Empty;
// });

app.UseFastEndpoints().UseSwaggerGen();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
//app.UseHttpsRedirection();
app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();