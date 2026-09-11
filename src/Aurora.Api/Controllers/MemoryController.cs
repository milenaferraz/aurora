using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aurora.Application.Interfaces;

namespace Aurora.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemoryController : ControllerBase
    {
        private readonly IMemoryProvider _memoryProvider;

        public MemoryController(IMemoryProvider memoryProvider)
        {
            _memoryProvider = memoryProvider ?? throw new ArgumentNullException(nameof(memoryProvider));
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<string>> Get(string key, CancellationToken cancellationToken)
        {
            var value = await _memoryProvider.GetAsync(key, cancellationToken);
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [HttpPost]
        public async Task<ActionResult> Set([FromBody] MemorySetRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.Key))
            {
                return BadRequest("Key is required");
            }
            await _memoryProvider.SetAsync(request.Key, request.Value, cancellationToken);
            return Ok();
        }

        [HttpDelete("{key}")]
        public async Task<ActionResult> Delete(string key, CancellationToken cancellationToken)
        {
            await _memoryProvider.RemoveAsync(key, cancellationToken);
            return NoContent();
        }
    }

    public class MemorySetRequest
    {
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}
