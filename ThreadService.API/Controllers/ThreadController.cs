﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThreadService.API.Services;
using ThreadService.API.Models;
using ThreadService.API.Context;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Confluent.Kafka;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ThreadService.API.Kafka;
using System.Diagnostics.CodeAnalysis;

namespace ThreadService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ExcludeFromCodeCoverage]
    public class ThreadController : ControllerBase
    {
        private readonly IThreadService _service;
        private readonly IKafkaProducer _producer;
        public ThreadController(IThreadService service, IKafkaProducer producer)
        {
            _service = service;
            _producer = producer;
        }

        [HttpGet]
        public async Task<List<Models.Thread>> Get() => 
            await _service.GetThreads();

        [HttpGet("GetThreadsByName/{name}")]
        public async Task<List<Models.Thread>> GetThreadsByName(string name) =>
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
            CreatedAtAction(nameof(Get), new { id = ((await _service.InsertThread(new Models.Thread { Name = thread.Name, Description = thread.Description,  Posts = 0 }))?.Id) 
                ?? throw new InvalidOperationException("Failed to insert the thread.") }, thread);

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<Models.Thread>> Update(string id, ThreadDTO thread, CancellationToken stoppingToken)
        {
            var t = await _service.GetThread(id);
            var oldname = t.Name;

            if (t is null)
            {
                return NotFound();
            }
            
            t.Name = thread.Name;
            t.Description = thread.Description;
            var th = await _service.UpdateThread(t);
            
            if (oldname != th?.Name)
            {
                _ = _producer.Produce(JsonSerializer.Serialize(new { t.Id, th?.Name }), stoppingToken);
            }

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
