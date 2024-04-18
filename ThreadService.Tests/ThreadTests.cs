using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThreadService.API;
using ThreadService.API.Context;

namespace ThreadService.Tests
{
    public class ThreadTests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;

        public ThreadTests(MongoDbFixture fixture)
        {
            _fixture = fixture;
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IMongoClient>();
                        services.AddSingleton<IMongoClient>(
                            (_) => _fixture.Client);
                    });
                });
            _client = appFactory.CreateClient();
        }
        [Fact]
        public async Task GetAllThreads_ShouldReturnThreads()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            await collection.InsertOneAsync(new API.Models.Thread { Name = "Test name", Description = "test description", Posts = 0 });

            // Act
            var res = await _client.GetAsync("/thread");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var threads = JsonSerializer.Deserialize<ICollection<API.Models.Thread>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Single(threads);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetAllThreads_NoThreadsInDb_ShouldReturnNoThreads()
        {
            // Arrange

            // Act
            var res = await _client.GetAsync("/thread");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var threads = JsonSerializer.Deserialize<ICollection<API.Models.Thread>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(threads);
            Assert.Empty(threads);
        }
        public void Dispose()
        {
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            collection.DeleteManyAsync(_ => true).Wait();
        }
    }
}
