using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadService.API.Context;
using ThreadService.API.Services;

namespace ThreadService.Tests
{
    public class ThreadServiceUnitTests
    {
        public ThreadServiceUnitTests()
        {

        }
        [Fact]
        public async Task GetThreadById_ShouldReturnThreadWithCorrectId()
        {
            // Arrange
            IThreadService _service = new API.Services.ThreadService(new TestDAL());

            // Act
            var t1 = await _service.GetThread("thread1");
            var t2 = await _service.GetThread("thread2");

            // Assert
            Assert.NotNull(t1);
            Assert.Equal("thread1", t1.Id);

            Assert.NotNull(t2);
            Assert.Equal("thread2", t2.Id);
        }
        [Fact]
        public async Task GetAllThreads_ShouldReturnThreads()
        {
            // Arrange
            IThreadService _service = new API.Services.ThreadService(new TestDAL());

            // Act
            var threads = await _service.GetThreads();

            // Assert
            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Equal(3, threads.Count);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetThreadByName_ShouldReturnThreadsThatContainName()
        {
            // Arrange
            IThreadService _service = new API.Services.ThreadService(new TestDAL());

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
        public async Task InsertOneThread_ShouldInsertThread()
        {
            // Arrange
            IThreadService _service = new API.Services.ThreadService(new TestDAL());
            API.Models.Thread thread = new() { Name = "Test name", Description = "test description", Posts = 0 };

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
            Assert.Equal(4, threads.Count);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateOneThread_ShouldUpdateThread()
        {
            // Arrange
            IThreadService _service = new API.Services.ThreadService(new TestDAL());

            // Act
            var t = await _service.GetThread("thread3");
            t.Name = "Updated Name";
            t.Description = "Updated Description";
            _ = await _service.UpdateThread(t);
            var threads = await _service.GetThreads();

            // Assert
            Assert.NotNull(t);
            Assert.Equal("thread3", t.Id);
            Assert.Equal("Updated Name", t.Name);
            Assert.Equal("Updated Description", t.Description);

            Assert.NotNull(threads);
            Assert.NotEmpty(threads);
            Assert.Equal(3, threads.Count);
            var resultItem = threads.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task DeleteOneThread_ShouldDeleteThread()
        {
            // Arrange
            IThreadService _service = new API.Services.ThreadService(new TestDAL());

            // Act
            await _service.DeleteThread("thread1");
            await _service.DeleteThread("thread2");
            await _service.DeleteThread("thread3");
            var threads = await _service.GetThreads();

            // Assert
            Assert.NotNull(threads);
            Assert.Empty(threads);
        }
        private class TestDAL : IThreadContext
        {
            readonly List<API.Models.Thread> _threads = new List<API.Models.Thread>
            {
                new() { Id = "thread1", Name = "Test", Description = "Test description", Posts = 2 },
                new() { Id = "thread2", Name = "Test2", Description = "Test description2", Posts = 0 },
                new() { Id = "thread3", Name = "Other", Description = "other", Posts = 0 }
            };

            Task<API.Models.Thread?> IThreadContext.CreateAsync(API.Models.Thread thread)
            {
                _threads.Add(thread);
                return Task.FromResult<API.Models.Thread?>(thread);
            }

            Task<API.Models.Thread?> IThreadContext.GetAsync(string id)
            {
                API.Models.Thread? thread = _threads.FirstOrDefault(thread => thread.Id == id);
                if (thread != null)
                {
                    return Task.FromResult<API.Models.Thread?>(thread);
                }
                else
                {
                    return Task.FromResult<API.Models.Thread?>(null);
                }
            }

            Task<List<API.Models.Thread>> IThreadContext.GetAsync()
            {
                return Task.FromResult(_threads);
            }

            Task<List<API.Models.Thread>> IThreadContext.GetAsyncNameSearch(string name)
            {
                var threads = _threads.Where(thread => thread.Name.Contains(name)).ToList();
                return Task.FromResult(threads);
            }

            Task IThreadContext.RemoveAsync(string id)
            {
                var threadToRemove = _threads.FirstOrDefault(thread => thread.Id == id);
                if (threadToRemove != null)
                {
                    _threads.Remove(threadToRemove);
                }
                return Task.CompletedTask;
            }

            Task<API.Models.Thread?> IThreadContext.UpdateAsync(API.Models.Thread thread)
            {
                API.Models.Thread? existingThread = _threads.FirstOrDefault(p => p.Id == thread.Id);
                if (existingThread != null)
                {
                    existingThread.Name = thread.Name;
                    existingThread.Description = thread.Description;
                    existingThread.Posts = thread.Posts;

                    return Task.FromResult<API.Models.Thread?>(existingThread);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
