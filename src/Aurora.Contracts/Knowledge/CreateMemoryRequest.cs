using System;
using System.Collections.Generic;

namespace Aurora.Contracts.Knowledge
{
    public sealed record CreateMemoryRequest
    {
        public string Title { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public string? Project { get; init; }
        public IReadOnlyCollection<string>? Tags { get; init; }
        public IReadOnlyCollection<string>? Relations { get; init; }
        public string? Source { get; init; }
        public decimal? Importance { get; init; }
    }
}
