using ThreadService.API.Context;

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

        public async Task<List<Models.Thread>> GetThreadsByName(string name)
        {
            return await _context.GetAsyncNameSearch(name);
        }

        public async Task<List<Models.Thread>> GetThreads()
        {
            return await _context.GetAsync();
        }

        public async Task<Models.Thread?> InsertThread(Models.Thread thread)
        {
            return await _context.CreateAsync(thread);
        }

        public async Task<Models.Thread?> UpdateThread(Models.Thread thread)
        {
            return await _context.UpdateAsync(thread);
        }

        public async Task DeleteThread(string id)
        {
            await _context.RemoveAsync(id);
        }
    }
}
