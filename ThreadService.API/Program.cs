using MongoDB.Driver;
using ThreadService.API.Services;
using Confluent.Kafka;
using ThreadService.API.Context;
using ThreadService.API.SeedData;
using ThreadService.API.Kafka;
using Prometheus;

namespace ThreadService.API
{
    public class Program
    {
        /*public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());*/
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMetrics();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database
            //builder.Services.Configure<ThreadDBSettings>(
            //    builder.Configuration.GetSection("ThreadDB"));

            var connString = builder.Configuration.GetConnectionString("MongoDB");
            builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connString));

            builder.Services.AddSingleton<IDataSeedingConfiguration, DataSeedingConfiguration>();

            builder.Services.AddSingleton<IThreadContext, ThreadContext>();

            builder.Services.AddSingleton<IThreadService, Services.ThreadService>();

            //var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("ThreadDB"));
            //var database = mongoClient.GetDatabase("ThreadDB");
            //builder.Services.AddSingleton<IMongoDatabase>(database);

            //Kafka producer
            var producerConfig = builder.Configuration.GetSection("ProducerConfig").Get<ProducerConfig>();
            var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            builder.Services.AddSingleton<IKafkaProducer>(_ => new KafkaProducer(producer, "updatethreadname"));

            //Kafka consumer
            var consumerConfig = builder.Configuration.GetSection("ConsumerConfig").Get<ConsumerConfig>();
            var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            //consumer.Subscribe("newpost");

            builder.Services.AddHostedService(sp =>
                new KafkaConsumer(sp.GetRequiredService<ILogger<KafkaConsumer>>(), consumer, sp.GetRequiredService<IThreadService>()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            //app.UseHttpsRedirection();
            app.UseHttpMetrics();
            app.UseMetricServer();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
