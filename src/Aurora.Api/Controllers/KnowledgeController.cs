using System;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Knowledge;
using Aurora.Contracts.Knowledge;
using Microsoft.AspNetCore.Mvc;

namespace Aurora.Api.Controllers
{
    [ApiController]
    [Route("api/v1/knowledge")]
    public class KnowledgeController : ControllerBase
    {
        private readonly IMemoryService _knowledgeService;

        public KnowledgeService(IMemoryService knowledgeService)
        {
            _knowledgeService = knowledgeService ?? throw new ArgumentNullException(nameof(knowledgeService));
        }

        [HttpPost("memories")]
        public async Task<ActionResult<MemoryResponse>> Create(
            [FromBody] CreateMemoryRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _knowledgeService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("memories/{id}")]
        public async Task<ActionResult<MemoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _knowledgeService.GetByIdAsync(id, cancellationToken);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPut("memories/{id}")]
        public async Task<ActionResult<MemoryResponse>> Update(Guid id,
            [FromBody] UpdateMemoryRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _knowledgeService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }
    }
}
