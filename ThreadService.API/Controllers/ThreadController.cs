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
        private readonly IThreadService _service;
        private readonly IKafkaProducer _producer;
        public ThreadController(IThreadService service, IKafkaProducer producer)
        {
            _service = service;
            _producer = producer;
        }

        [HttpGet("KafkaTest")]
        public async Task<ActionResult<string>> KafkaTest(CancellationToken stoppingToken)
        {
            List<string> strings = ["NEWMESSAGES", "NEWMESSAGES222222222222222"];
            await _producer.ProduceMultiple(strings, stoppingToken);
            return "Produced??";
        }
        [HttpGet("KafkaTest2")]
        public async Task<ActionResult<string>> KafkaTest2(string message, CancellationToken stoppingToken)
        {
            await _producer.Produce(message, stoppingToken);
            return "Produced??";
        }

        [HttpGet]
        public async Task<List<Models.Thread>> Get() => 
            await _service.GetThreads();

        [HttpGet("GetThreadByName")]
        public async Task<List<Models.Thread>> GetThreadByName(string name) =>
            await _service.GetThreadsByName(name);

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
        public async Task<ActionResult<Models.Thread>> Post(ThreadDTO thread) =>
            CreatedAtAction(nameof(Get), new { id = ((await _service.InsertThread(new Models.Thread { Description = thread.Description, Name = thread.Name, Posts = 0 }))?.Id) 
                ?? throw new InvalidOperationException("Failed to insert the thread.") }, thread);

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<Models.Thread>> Update(string id, ThreadDTO thread)
        {
            var t = await _service.GetThread(id);

            if (t is null)
            {
                return NotFound();
            }

            var th = await _service.UpdateThread(new Models.Thread { Id = t.Id, Description = thread.Description, Name = thread.Name, Posts = t.Posts });
            
            if (th is null)
            {
                return NotFound();
            }

            return th;
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
