using BotTemplateWebApi.App.Options;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Register options
services.Configure<ApplicationConfiguration>(builder.Configuration);
services.Configure<ApplicationConfiguration.BotConfiguration>(builder.Configuration.GetSection("Bot"));

// Add services to the container.

services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();