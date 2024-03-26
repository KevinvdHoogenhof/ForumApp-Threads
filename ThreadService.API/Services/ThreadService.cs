using ThreadService.API.Models;
using ThreadService.API.Context;
using System.Threading;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ThreadService.API.Services
{
    public class ThreadService //: IThreadService
    {
        private readonly IMongoCollection<Models.Thread> _threads;
        public ThreadService(IOptions<ThreadDBSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _threads = mongoDatabase.GetCollection<Models.Thread>(settings.Value.CollectionName);
        }
        public async Task<Models.Thread?> GetThread(string id)
        {
            return await _threads.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Models.Thread>> GetThreads()
        {
            return await _threads.Find(_ => true).ToListAsync();
        }

        public async Task InsertThread(Models.Thread thread)
        {
            await _threads.InsertOneAsync(thread);
        }

        public async Task UpdateThread(Models.Thread thread)
        {
            await _threads.ReplaceOneAsync(x => x.Id == thread.Id, thread);
        }

        public async Task DeleteThread(string id)
        {
            await _threads.DeleteOneAsync(x => x.Id == id);
        }
        /*
        private readonly IThreadContext _context;
        public ThreadService(IOptions<ThreadDBSettings> settings)
        {
            _context = new ThreadContext(settings);
        }
        public async Task<Models.Thread?> GetThread(string id)
        {
            return await _context.GetAsync(id);
        }

        public async Task<List<Models.Thread>> GetThreads()
        {
            return await _context.GetAsync();
        }

        public async Task InsertThread(Models.Thread thread)
        {
            await _context.CreateAsync(thread);
        }

        public async Task UpdateThread(Models.Thread thread)
        {
            await _context.UpdateAsync(thread);
        }

        public async Task DeleteThread(string id)
        {
            await _context.RemoveAsync(id);
        }*/
    }
}
