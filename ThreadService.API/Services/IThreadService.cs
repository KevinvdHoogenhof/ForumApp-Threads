using ThreadService.API.Models;
namespace ThreadService.API.Services
{
    public interface IThreadService
    {
        public Task<Models.Thread?> GetThread(string id);
        public Task<List<Models.Thread>> GetThreads();
        public Task InsertThread(Models.Thread thread);
        public Task UpdateThread(Models.Thread thread);
        public Task DeleteThread(string id);
    }
}
