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
using ThreadService.API.Services;

namespace ThreadService.Tests
{
    public class ThreadServiceTests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;
        private readonly IThreadService _service;

        public ThreadServiceTests(MongoDbFixture fixture)
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
            _service = new API.Services.ThreadService(new ThreadContext(_fixture.Client));
        }
        [Fact]
        public async Task GetThreadById_ShouldReturnThreadWithCorrectId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            API.Models.Thread thread1 = new API.Models.Thread { Name = "Test name", Description = "test description", Posts = 0 };
            API.Models.Thread thread2 = new API.Models.Thread { Name = "Test name2", Description = "test description2", Posts = 0 };
            await collection.InsertOneAsync(thread1);
            await collection.InsertOneAsync(thread2);

            // Act
            var t1 = await _service.GetThread(thread1.Id);
            var t2 = await _service.GetThread(thread2.Id);

            // Assert
            Assert.NotNull(t1);
            Assert.Equal(thread1.Id, t1.Id);
            Assert.Equal(thread1.Name, t1.Name);
            Assert.Equal(thread1.Description, t1.Description);
            Assert.Equal(thread1.Posts, t1.Posts);

            Assert.NotNull(t2);
            Assert.Equal(thread2.Id, t2.Id);
            Assert.Equal(thread2.Name, t2.Name);
            Assert.Equal(thread2.Description, t2.Description);
            Assert.Equal(thread2.Posts, t2.Posts);
        }
        [Fact]
        public async Task GetThreadByName_ShouldReturnThreadWithSameName()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            API.Models.Thread thread1 = new API.Models.Thread { Name = "Test name", Description = "test description", Posts = 0 };
            API.Models.Thread thread2 = new API.Models.Thread { Name = "Test name2", Description = "test description2", Posts = 0 };
            API.Models.Thread thread3 = new API.Models.Thread { Name = "Different Name", Description = "test description3", Posts = 0 };
            await collection.InsertOneAsync(thread1);
            await collection.InsertOneAsync(thread2);
            await collection.InsertOneAsync(thread3);

            // Act
            var threads = await _service.GetThreadsByName("Test");

            // Assert
            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Equal(2, threads.Count);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Contains("Test", resultItem.Name);
        }
        [Fact]
        public async Task GetAllThreads_ShouldReturnThreads()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            await collection.InsertOneAsync(new API.Models.Thread { Name = "Test name", Description = "test description", Posts = 0 });

            // Act
            var threads = await _service.GetThreads();

            // Assert
            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Single(threads);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task InsertOneThread_ShouldInsertThread()
        {
            // Arrange
            API.Models.Thread thread = new API.Models.Thread { Name = "Test name", Description = "test description", Posts = 0 };

            // Act
            var t = await _service.InsertThread(thread);
            var threads = await _service.GetThreads();

            // Assert
            Assert.NotNull(t);
            Assert.Equal(thread.Id, t.Id);
            Assert.Equal(thread.Name, t.Name);
            Assert.Equal(thread.Description, t.Description);
            Assert.Equal(thread.Posts, t.Posts);

            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Single(threads);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateOneThread_ShouldUpdateThread()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("ThreadDB");
            var collection = _db.GetCollection<API.Models.Thread>("Threads");
            API.Models.Thread thread = new API.Models.Thread { Name = "Test name", Description = "test description", Posts = 0 };
            await collection.InsertOneAsync(thread);

            // Act
            var t = await _service.GetThread(thread.Id);
            t.Name = "Updated Name";
            t.Description = "Updated Description";
            _ = await _service.UpdateThread(t);
            var threads = await _service.GetThreads();

            // Assert
            Assert.NotNull(t);
            Assert.Equal(thread.Id, t.Id);
            Assert.Equal("Updated Name", t.Name);
            Assert.Equal("Updated Description", t.Description);
            Assert.Equal(thread.Posts, t.Posts);

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
            API.Models.Thread thread = new API.Models.Thread { Name = "Test name", Description = "test description", Posts = 0 };
            await collection.InsertOneAsync(thread);

            // Act
            await _service.DeleteThread(thread.Id);
            var threads = await _service.GetThreads();

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
