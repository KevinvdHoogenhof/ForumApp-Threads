using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;
using ThreadService.API;
using ThreadService.API.SeedData;

namespace ThreadService.Tests
{
    public class APITests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;

        public APITests(MongoDbFixture fixture)
        {
            _fixture = fixture;
            var dataSeedingConfig = new DataSeedingConfiguration { SeedDataEnabled = false };
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IMongoClient>();
                        services.AddSingleton<IMongoClient>(
                            (_) => _fixture.Client);
                        services.RemoveAll<IDataSeedingConfiguration>();
                        services.AddSingleton<IDataSeedingConfiguration>(dataSeedingConfig);
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
        [Fact]
        public async Task GetThreadsByName_ShouldReturnThreadsThatContainsName()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            API.Models.Thread thread1 = new() { Name = "Test name", Description = "test description", Posts = 0 };
            API.Models.Thread thread2 = new() { Name = "Test name2", Description = "test description2", Posts = 0 };
            API.Models.Thread thread3 = new() { Name = "Different Name", Description = "test description3", Posts = 0 };
            await collection.InsertOneAsync(thread1);
            await collection.InsertOneAsync(thread2);
            await collection.InsertOneAsync(thread3);

            // Act
            var res = await _client.GetAsync("/thread/getthreadsbyname/Test");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var threads = JsonSerializer.Deserialize<ICollection<API.Models.Thread>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Equal(2, threads.Count);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Contains("Test", resultItem.Name);
        }
        [Fact]
        public async Task PostThread_ShouldPostThread()
        {
            //Arrange
            API.Models.ThreadDTO thread = new() { Name = "Test name", Description = "test description" };
            
            // Act
            var res = await _client.PostAsync("/thread/", new StringContent(JsonSerializer.Serialize(thread), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var t = JsonSerializer.Deserialize<API.Models.Thread>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var res2 = await _client.GetAsync("/thread");
            res2.EnsureSuccessStatusCode();
            var content2 = await res2.Content.ReadAsStringAsync();
            var threads = JsonSerializer.Deserialize<ICollection<API.Models.Thread>>(content2, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(t);
            Assert.Equal(thread.Name, t.Name);
            Assert.Equal(thread.Description, t.Description);
            Assert.Equal(0, t.Posts);

            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Single(threads);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateThread_ShouldUpdateThread()
        {
            //Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            API.Models.Thread thread = new() { Name = "Test name", Description = "test description", Posts = 0 };
            await collection.InsertOneAsync(thread);

            API.Models.ThreadDTO updatedthread = new() { Name = "Updated test name", Description = "Updated test description" };

            // Act
            var res = await _client.PutAsync($"/thread/{thread.Id}", new StringContent(JsonSerializer.Serialize(updatedthread), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var t = JsonSerializer.Deserialize<API.Models.Thread>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var res2 = await _client.GetAsync("/thread");
            res2.EnsureSuccessStatusCode();
            var content2 = await res2.Content.ReadAsStringAsync();
            var threads = JsonSerializer.Deserialize<ICollection<API.Models.Thread>>(content2, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(t);
            Assert.Equal(updatedthread.Name, t.Name);
            Assert.Equal(updatedthread.Description, t.Description);
            Assert.Equal(0, t.Posts);

            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Single(threads);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task DeleteOneThread_ShouldDeleteThread()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            API.Models.Thread thread = new() { Name = "Test name", Description = "test description", Posts = 0 };
            await collection.InsertOneAsync(thread);

            // Act
            var res = await _client.DeleteAsync($"/thread/{thread.Id}");
            res.EnsureSuccessStatusCode();

            var res2 = await _client.GetAsync("/thread");
            res2.EnsureSuccessStatusCode();
            var content = await res2.Content.ReadAsStringAsync();
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
