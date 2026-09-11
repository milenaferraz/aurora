using System;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Contracts.Knowledge;
using Aurora.Domain.Knowledge;

namespace Aurora.Application.Knowledge
{
    public interface IMemoryService
    {
        Task<MemoryResponse> CreateAsync(CreateMemoryRequest request, CancellationToken cancellationToken = default);
        Task<MemoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MemoryResponse> UpdateAsync(Guid id, UpdateMemoryRequest request, CancellationToken cancellationToken = default);
    }
}
