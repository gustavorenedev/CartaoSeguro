using System.Text.Json;
using CartaoSeguro.Notifiers.DTOs;
using CartaoSeguro.Notifiers.Email.Service;
using CartaoSeguro.Notifiers.Email.Service.Interface;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CartaoSeguro.Notifiers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _cardTopicName;
    private readonly string _kafkaBootstrapServers;
    private readonly string _notifierConsumeGroupName;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _cardTopicName = _configuration["CardSettings:CardTopicName"];
        _kafkaBootstrapServers = _configuration["CardSettings:KafkaBootstrapServer"];
        _notifierConsumeGroupName = _configuration["CardSettings:NotifierConsumeGroupName"];
    }

    /// <summary>
    /// Lopping process
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started and waiting for messages...");

        IEmailService emailService = new EmailService(_configuration);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Card notifier running at: {time}", DateTimeOffset.Now);

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaBootstrapServers,
                GroupId = _notifierConsumeGroupName,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_cardTopicName);

                _logger.LogInformation("Subscribe in topic {topic}", _cardTopicName);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);

                            _logger.LogInformation("Message received: {message}", consumeResult.Message.Value);

                            CardAndUser cardAndUserRequest = JsonSerializer.Deserialize<CardAndUser>(consumeResult.Message.Value)!;

                            await emailService.SendEmailAsync(cardAndUserRequest);

                            Console.WriteLine($"Processed message: {consumeResult.Message.Value}");
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError("Error consuming message: {error}", e.Error.Reason);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Worker cancelled. Closing consumer...");
                    consumer.Close();
                }
            }
        }
    }
}