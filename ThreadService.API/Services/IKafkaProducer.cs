namespace ThreadService.API.Services
{
    public interface IKafkaProducer
    {
        Task Produce(IReadOnlyCollection<string> test, CancellationToken cancellationToken);
    }
}
