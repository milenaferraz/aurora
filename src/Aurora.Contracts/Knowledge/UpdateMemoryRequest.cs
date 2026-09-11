using System;
using System.Collections.Generic;

namespace Aurora.Contracts.Knowledge
{
    public sealed record UpdateMemoryRequest
    {
        public string? Title { get; init; }
        public string? Content { get; init; }
        public string? Project { get; init; }
        public IReadOnlyCollection<string>? Tags { get; init; }
        public IReadOnlyCollection<string>? Relations { get; init; }
        public decimal? Importance { get; init; }
    }
}
