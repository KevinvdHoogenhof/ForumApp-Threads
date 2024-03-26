using ThreadService.API.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace ThreadService.API.Context
{
    public class ThreadContext : IThreadContext
    {
        private readonly IMongoCollection<Models.Thread> _threads;
        public ThreadContext(IOptions<ThreadDBSettings> threaddbsettings)
        {
            var mongoClient = new MongoClient(threaddbsettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(threaddbsettings.Value.DatabaseName);
            _threads = mongoDatabase.GetCollection<Models.Thread>(threaddbsettings.Value.CollectionName);
        }
        public async Task<Models.Thread?> GetAsync(string id)
        {
            return await _threads.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Models.Thread>> GetAsync()
        {
            return await _threads.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(Models.Thread thread)
        {
            await _threads.InsertOneAsync(thread);
        }

        public async Task UpdateAsync(Models.Thread thread)
        {
            await _threads.ReplaceOneAsync(x => x.Id == thread.Id, thread);
        }

        public async Task RemoveAsync(string id)
        {
            await _threads.DeleteOneAsync(x => x.Id == id);
        }
    }
}
