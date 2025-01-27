using System.Text.Json;
using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.MessagePublisher.Service.Interface;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace CartaoSeguro.Application.MessagePublisher.Service;

public class KafkaMessagePublisher : IMessagePublisher
{
    private readonly IConfiguration _configuration;
    private readonly string _kafkaBootstrapServers;
    private readonly string _cardTopicName;

    public KafkaMessagePublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        _kafkaBootstrapServers = _configuration["CardSettings:KafkaBootstrapServer"];
        _cardTopicName = _configuration["CardSettings:CardTopicName"];
    }

    public async Task PublishAsync(CardAndUserRequest request)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaBootstrapServers
        };

        string cardAndUserConvertedToJson = JsonSerializer.Serialize(request);

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            await producer.ProduceAsync(_cardTopicName, new Message<Null, string> { Value = cardAndUserConvertedToJson });
        }
    }
}
