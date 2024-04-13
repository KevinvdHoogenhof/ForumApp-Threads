using ThreadService.API.Models;
using System.Collections.Generic;

namespace ThreadService.API.Context
{
    public interface IThreadContext
    {
        public Task<Models.Thread?> GetAsync(string id);
        public Task<List<Models.Thread>> GetAsync();
        public Task<List<Models.Thread>> GetAsyncNameSearch(string name);
        public Task<Models.Thread?> CreateAsync(Models.Thread thread);
        public Task<Models.Thread?> UpdateAsync(Models.Thread thread);
        public Task RemoveAsync(string id);
    }
}
