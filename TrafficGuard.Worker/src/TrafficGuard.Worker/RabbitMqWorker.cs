using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TrafficGuard.Application.DTOs;
using TrafficGuard.Application.DTOs.SpeedTelemetry;
using TrafficGuard.Application.Features.Telemetry.Commands.ProcessTelemetry;

namespace TrafficGuard.Worker;

public class RabbitMqWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _queueName;

    public RabbitMqWorker(IServiceProvider serviceProvider, ILogger<RabbitMqWorker> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
        _queueName = _configuration["RabbitMq:QueueName"] ?? "radares";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:Host"],
            UserName = _configuration["RabbitMq:Username"],
            Password = _configuration["RabbitMq:Password"]
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                _logger.LogInformation(" [*] Waiting for telemetry in queue '{Queue}'...", _queueName);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        var dto = JsonConvert.DeserializeObject<SpeedTelemetryDTO>(message);

                        if (dto != null)
                        {

                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                                var plateClean = dto.LicensePlate.Replace("-", "").Trim().ToUpper();

                                var dateUtc = dto.Timestamp.ToUniversalTime();

                                var command = new ProcessTelemetryCommand(
                                    plateClean,
                                    dto.Speed,
                                    dto.RoadName,
                                    dateUtc 
                                );

                                await mediator.Send(command, stoppingToken);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                    }
                };

                await channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("RabbitMQ connection failed. Retrying in 5s... Error: {Error}", ex.Message);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}