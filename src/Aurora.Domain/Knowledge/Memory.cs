using System;

namespace Aurora.Domain.Knowledge
{
    public class Memory
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = default!;
        public MemoryType Type { get; init; }
        public string Content { get; init; } = default!;
        public string? Project { get; init; }
        public IReadOnlyCollection<string>? Tags { get; init; }
        public IReadOnlyCollection<string>? Relations { get; init; }
        public string? Source { get; init; }
        public decimal Importance { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }

        public Memory(Guid id, string title, MemoryType type, string content,
                      string? project, IReadOnlyCollection<string>? tags,
                      IReadOnlyCollection<string>? relations, string? source,
                      decimal importance, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Title = title;
            Type = type;
            Content = content;
            Project = project;
            Tags = tags;
            Relations = relations;
            Source = source;
            Importance = importance;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
