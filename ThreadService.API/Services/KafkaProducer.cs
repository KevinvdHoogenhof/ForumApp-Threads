using Confluent.Kafka;

namespace ThreadService.API.Services
{
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, int> _producer;
        private readonly string _topic;

        public KafkaProducer(IProducer<string, int> producer, string topic)
        {
            _producer = producer;
            _topic = topic;
        }

        public async Task Produce(IReadOnlyCollection<string> test, CancellationToken cancellationToken)
        {
            var tasks = test
                .Select(t =>
                {
                    var message = new Message<string, int>()
                    {
                        Key = t,
                        Value = 10,
                    };

                    return _producer.ProduceAsync(_topic, message, cancellationToken);
                })
                .ToArray();

            await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
