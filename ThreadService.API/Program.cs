using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using ThreadService.API.Models;
using ThreadService.API.Services;
using Confluent.Kafka;
using ThreadService.API.Context;

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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database
            //builder.Services.Configure<ThreadDBSettings>(
            //    builder.Configuration.GetSection("ThreadDB"));

            var connString = builder.Configuration.GetConnectionString("MongoDB");
            builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connString));

            builder.Services.AddSingleton<IThreadContext, ThreadContext>();

            builder.Services.AddSingleton<IThreadService, Services.ThreadService>();

            //var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("ThreadDB"));
            //var database = mongoClient.GetDatabase("ThreadDB");
            //builder.Services.AddSingleton<IMongoDatabase>(database);

            //Kafka producer
            var producerConfig = builder.Configuration.GetSection("ProducerConfig").Get<ProducerConfig>();
            var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            builder.Services.AddSingleton<IKafkaProducer>(_ => new KafkaProducer(producer, "updatethreadname"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
