using System.Reflection;
using BotFramework.Db;
using BotFramework.Extensions;
using BotFramework.Options;
using BotFramework.Other;
using BotTemplateWebApi.App.Options;
using BotTemplateWebApi.Extentsions;
using BotTemplateWebApi.Resources;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Initialize

// Register options
services.AddLogging();
services.Configure<ApplicationConfiguration>(builder.Configuration);
services.Configure<BotConfiguration>(builder.Configuration.GetSection("Bot"));
services.Configure<BotOptions>(builder.Configuration.GetSection("BotOptions"));
var botConfig = builder.Configuration.GetSection("Bot").Get<BotConfiguration>();
BotResources botResources = services.ConfigureBotResources(botConfig.ResourcesFilePath);
services.AddBot(botConfig);

// Add services to the container.
services.AddMapster(Assembly.GetExecutingAssembly());
services.AddControllers().AddNewtonsoftJson();
services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();