using CartaoSeguro.Notifiers.DTOs;
using CartaoSeguro.Notifiers.Email.Service;
using CartaoSeguro.Notifiers.Email.Service.Interface;
using Confluent.Kafka;
using System.Text.Json;

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
        IEmailService emailService = new EmailService();
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Order notifier running at: {time}", DateTimeOffset.Now);

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaBootstrapServers,
                GroupId = _notifierConsumeGroupName,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_cardTopicName);

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

                            CardAndUser cardAndUserRequest = JsonSerializer.Deserialize<CardAndUser>(consumeResult.Message.Value)!;

                            await emailService.SendEmailAsync(cardAndUserRequest);

                            Console.WriteLine($"Mensagem recebida: {consumeResult.Message.Value}");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Erro ao consumir a mensagem: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }
        }
    }
}
