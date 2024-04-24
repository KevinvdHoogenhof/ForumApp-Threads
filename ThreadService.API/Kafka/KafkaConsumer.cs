using Confluent.Kafka;
using System.Text.Json;
using ThreadService.API.Services;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ThreadService.API.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _log;
        private readonly IConsumer<Null, string> _consumer;
        private readonly IThreadService _service;

        public KafkaConsumer(ILogger<KafkaConsumer> log, IConsumer<Null, string> consumer, IThreadService service)
        {
            _log = log;
            _consumer = consumer;
            _service = service;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _consumer.Subscribe("newpost");

                    var consumeResult = _consumer.Consume(stoppingToken);
                    var mv = consumeResult.Message.Value;
                    _log.LogInformation(mv);

                    try
                    {
                        var t = JsonSerializer.Deserialize<ThreadIdPostId>(mv);
                        var p = t != null ? await _service.GetThread(t.ThreadId) : null;
                        p.Posts = t.Posts;
                        await _service.UpdateThread(p);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                    }

                    if (i++ % 1000 == 0)
                    {
                        _consumer.Commit();
                    }
                }
                catch (ConsumeException ex)
                {
                    _log.LogInformation($"Error consuming message: {ex.Error.Reason}");
                    
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "An unexpected error occurred while consuming messages.");
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Dispose();
            base.Dispose();
        }
        private class ThreadIdPostId
        {
            public string ThreadId { get; set; } = null!;
            public int Posts { get; set; } = 0;
        }
    }
}
