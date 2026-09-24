using Confluent.Kafka;

namespace app_pm_back.Services;

public class KafkaProducerService : IDisposable
{
    public readonly IProducer<string, string> _producer;
    
    public KafkaProducerService(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            Acks = Acks.All // garante que a mensagem foi persistida no Kafka
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    
    public async Task ProduceAsync(string topic, Message<string, string> message)
    {
        await _producer.ProduceAsync(topic, message);
    }

    public void Dispose()
    {
        // garante que mensagens em buffer sejam enviadas antes de a aplicação desligar
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}