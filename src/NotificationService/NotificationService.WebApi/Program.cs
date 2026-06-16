using NotificationService.WebApi.Hubs;
using NotificationService.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddHostedService<KafkaConsumerHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<NotificationHub>("/notificationHub");

app.MapGet("/", () => "Notification Service is running and listening to Kafka...");

app.Run();
