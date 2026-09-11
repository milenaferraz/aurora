using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Domain.Knowledge;

namespace Aurora.Infrastructure.Knowledge
{
    public interface IVaultRepository
    {
        Task<MemoryFile> CreateAsync(Memory memory, string relativePath, CancellationToken cancellationToken = default);
        Task<MemoryFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MemoryFile> UpdateAsync(Memory memory, string relativePath, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<MemoryFile>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
