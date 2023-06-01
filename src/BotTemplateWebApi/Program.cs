using System.Reflection;
using BotFramework.Db;
using BotFramework.Extensions;
using BotFramework.Middleware;
using BotFramework.Options;
using BotFramework.Repository;
using BotTemplateWebApi.App.Options;
using BotTemplateWebApi.Extentsions;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Initialize

// Register options
services.Configure<ApplicationConfiguration>(builder.Configuration);
services.Configure<BotConfiguration>(builder.Configuration.GetSection("Bot"));
var botConfig = builder.Configuration.GetSection("Bot").Get<BotConfiguration>();
services.AddBot(botConfig);

// Add services to the container.
string connection = "Host=127.0.0.1;Port=5432;Database=bot_template_test;Username=postgres;Password=123";
services.AddDbContext<BotDbContext>(options =>
{
    options.UseNpgsql(connection);
});
services.AddTransient<IBaseBotRepository, BaseBotRepository>();
services.AddMapster(Assembly.GetExecutingAssembly());
//services.AddSingleton<IBotSingleton, BotSingleton>();
services.AddControllers().AddNewtonsoftJson();
services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


var app = builder.Build();

var serviceScope = app.Services.CreateScope();
var serviceProvider = serviceScope.ServiceProvider;
using (BotDbContext botDbContext = serviceProvider.GetRequiredService<BotDbContext>())
{
    await botDbContext.Database.MigrateAsync();
}

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