using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThreadService.API.Services;
using ThreadService.API.Models;
using ThreadService.API.Context;
using Microsoft.Extensions.Options;

namespace ThreadService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThreadController : ControllerBase
    {
        private readonly Services.ThreadService _service;
        public ThreadController(Services.ThreadService service) =>
            _service = service;

        //public ThreadController(IOptions<ThreadDBSettings> settings)
        //{
        //    _service = new Services.ThreadService(settings);
        //}

        [HttpGet]
        public async Task<List<Models.Thread>> Get() => 
            await _service.GetThreads();
        //{
             //return Ok(await _service.GetThreads());
        //}

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Models.Thread>> Get(string id)
        {
            var thread = await _service.GetThread(id);

            if (thread is null)
            {
                return NotFound();
            }

            return thread;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Models.Thread thread)
        {
            await _service.InsertThread(thread);
            return CreatedAtAction(nameof(Get), new { id = thread.Id },thread);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Models.Thread thread)
        {
            var t = await _service.GetThread(id);

            if (t is null)
            {
                return NotFound();
            }

            await _service.UpdateThread(thread);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var t = await _service.GetThread(id);

            if (t is null)
            {
                return NotFound();
            }

            await _service.DeleteThread(id);

            return NoContent();
        }
    }
}
