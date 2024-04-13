using ThreadService.API.Models;
using ThreadService.API.Context;
using System.Threading;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ThreadService.API.Services
{
    public class ThreadService : IThreadService
    {
        private readonly IThreadContext _context;
        public ThreadService(IThreadContext context)
        {
            _context = context;
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
        }
    }
}
