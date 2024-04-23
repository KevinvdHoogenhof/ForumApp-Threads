namespace ThreadService.API.Services
{
    public interface IThreadService
    {
        public Task<Models.Thread?> GetThread(string id);
        public Task<List<Models.Thread>> GetThreads();
        public Task<List<Models.Thread>> GetThreadsByName(string name);
        public Task<Models.Thread?> InsertThread(Models.Thread thread);
        public Task<Models.Thread?> UpdateThread(Models.Thread thread);
        public Task DeleteThread(string id);
    }
}
