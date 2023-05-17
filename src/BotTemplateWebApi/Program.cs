using System.Reflection;
using BotFramework.Db;
using BotFramework.Middleware;
using BotFramework.Repository;
using BotTemplateWebApi.App.Options;
using BotTemplateWebApi.Extentsions;
using BotTemplateWebApi.Interfaces.IServices;
using BotTemplateWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Initialize

// Register options
services.Configure<ApplicationConfiguration>(builder.Configuration);
services.Configure<ApplicationConfiguration.BotConfiguration>(builder.Configuration.GetSection("Bot"));

// Add services to the container.
services.AddDbContext<BotDbContext>(options =>
{
    options.UseNpgsql("Host=127.0.0.1;Port=5432;Database=marathon_bot_framework_test;Username=postgres;Password=123");
});
services.AddTransient<IBaseBotRepository, BaseBotRepository>();
services.AddMapster(Assembly.GetExecutingAssembly());
services.AddSingleton<IBotSingleton, BotSingleton>();
services.AddControllers().AddNewtonsoftJson();
services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


var app = builder.Build();

app.Services.GetRequiredService<IBotSingleton>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<BotControllerMiddleware>();
app.UseMiddleware<TestBotControllerMiddleware>();
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();



app.Run();