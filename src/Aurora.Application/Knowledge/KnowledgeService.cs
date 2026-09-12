using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Knowledge;
using Aurora.Contracts.Knowledge;
using Aurora.Domain.Knowledge;
using Aurora.Infrastructure.Knowledge;

namespace Aurora.Application.Knowledge
{
    public class KnowledgeService : IMemoryService
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly IMarkdownGenerator _markdownGenerator;
        private readonly ISlugGenerator _slugGenerator;

        public KnowledgeService(IVaultRepository vaultRepository,
                                IMarkdownGenerator markdownGenerator,
                                ISlugGenerator slugGenerator)
        {
            _vaultRepository = vaultRepository;
            _markdownGenerator = markdownGenerator;
            _slugGenerator = slugGenerator;
        }

        public async Task<MemoryResponse> CreateAsync(CreateMemoryRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request (basic)
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required");
            if (string.IsNullOrWhiteSpace(request.Type))
                throw new ArgumentException("Type is required");
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Content is required");

            // Normalize title
            var title = request.Title.Trim();
            if (title.Length < 3)
                throw new ArgumentException("Title must be at least 3 characters");
            if (title.Length > 150)
                throw new ArgumentException("Title must not exceed 150 characters");

            // Validate Type
            if (!Enum.TryParse<MemoryDomain.MemoryType>(request.Type, true, out var memoryType))
                throw new ArgumentException($"Invalid memory type: {request.Type}");

            // Normalize content
            var content = request.Content.Trim();
            if (content.Length < 3)
                throw new ArgumentException("Content must be at least 3 characters");
            // We'll allow up to 50000 as per spec, but not enforce strictly for now

            // Normalize project
            var project = request.Project?.Trim();

            // Normalize tags
            IReadOnlyCollection<string>? tags = null;
            if (request.Tags != null && request.Tags.Any())
            {
                tags = request.Tags
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim().ToLowerInvariant())
                    .Distinct()
                    .ToList();
            }

            // Normalize relations
            IReadOnlyCollection<string>? relations = null;
            if (request.Relations != null && request.Relations.Any())
            {
                relations = request.Relations
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r.Trim())
                    .Distinct()
                    .ToList();
            }

            var source = request.Source?.Trim() ?? "hermes";
            var importance = request.Importance ?? 0.5m;
            if (importance < 0m || importance > 1m)
                throw new ArgumentException("Importance must be between 0 and 1");

            var id = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var memory = new MemoryDomain.Memory(
                id, title, memoryType, content, project, tags, relations, source,
                importance, now, now);

            // Determine folder based on type
            string folder = memoryType switch
            {
                MemoryDomain.MemoryType.Decision => "03-Decisions",
                MemoryDomain.MemoryType.Fact => "01-Memory/Facts",
                MemoryDomain.MemoryType.Project => "02-Projects",
                MemoryDomain.MemoryType.Person => "01-Memory/People",
                MemoryDomain.MemoryType.Preference => "01-Memory/Preferences",
                MemoryDomain.MemoryType.Knowledge => "04-Knowledge",
                MemoryDomain.MemoryType.Meeting => "08-Meetings",
                MemoryDomain.MemoryType.Agent => "05-Agents",
                MemoryDomain.MemoryType.Incident => "09-Incidents",
                MemoryDomain.MemoryType.Note => "00-Inbox",
                _ => "00-Inbox"
            };

            // Generate slug from title
            var slug = _slugGenerator.Generate(title);
            // Ensure slug not empty
            if (string.IsNullOrWhiteSpace(slug))
                slug = "memory";

            // Build relative path
            var relativePath = Path.Combine(folder, $"{slug}.md");
            // We'll let VaultRepository handle path security and collision

            var memoryFile = await _vaultRepository.CreateAsync(memory, relativePath, cancellationToken);

            return new MemoryResponse
            {
                Id = memory.Id,
                Title = memory.Title,
                Type = memory.Type.ToString(),
                Content = memory.Content,
                Project = memory.Project,
                Tags = memory.Tags,
                Relations = memory.Relations,
                Source = memory.Source,
                Importance = memory.Importance,
                CreatedAt = memory.CreatedAt,
                UpdatedAt = memory.UpdatedAt
            };
        }

        public async Task<MemoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var memoryFile = await _vaultRepository.GetByIdAsync(id, cancellationToken);
            if (memoryFile == null)
                return null;
            return new MemoryResponse
            {
                Id = memoryFile.Id,
                Title = memoryFile.Title,
                Type = memoryFile.Type.ToString(),
                Content = memoryFile.Content,
                Project = memoryFile.Project,
                Tags = memoryFile.Tags,
                Relations = memoryFile.Relations,
                Source = memoryFile.Source,
                Importance = memoryFile.Importance,
                CreatedAt = memoryFile.CreatedAt,
                UpdatedAt = memoryFile.UpdatedAt
            };
        }

        public async Task<MemoryResponse> UpdateAsync(Guid id, UpdateMemoryRequest request, CancellationToken cancellationToken = default)
        {
            // First get existing memory to preserve fields not being updated
            var existing = await GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Memory with id {id} not found");

            // Build updated memory
            var title = string.IsNullOrWhiteSpace(request.Title) ? existing.Title : request.Title.Trim();
            var content = string.IsNullOrWhiteSpace(request.Content) ? existing.Content : request.Content.Trim();
            var project = string.IsNullOrWhiteSpace(request.Project) ? existing.Project : request.Project.Trim();
            IReadOnlyCollection<string>? tags = existing.Tags;
            if (request.Tags != null)
            {
                tags = request.Tags
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim().ToLowerInvariant())
                    .Distinct()
                    .ToList();
            }
            IReadOnlyCollection<string>? relations = existing.Relations;
            if (request.Relations != null)
            {
                relations = request.Relations
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r.Trim())
                    .Distinct()
                    .ToList();
            }
            var source = string.IsNullOrWhiteSpace(request.Source) ? existing.Source : request.Source.Trim();
            var importance = request.Importance ?? existing.Importance;
            // Type cannot be updated via this endpoint (could be added later)
            var type = Enum.Parse<MemoryDomain.MemoryType>(existing.Type);

            var now = DateTime.UtcNow;
            var updatedMemory = new MemoryDomain.Memory
            (
                id,
                title,
                type,
                content,
                project,
                tags,
                relations,
                source,
                importance,
                existing.CreatedAt, // preserve original created
                now
            );

            // Determine folder based on type (should be same as existing)
            string folder = type switch
            {
                MemoryDomain.MemoryType.Decision => "03-Decisions",
                MemoryDomain.MemoryType.Fact => "01-Memory/Facts",
                MemoryDomain.MemoryType.Project => "02-Projects",
                MemoryDomain.MemoryType.Person => "01-Memory/People",
                MemoryDomain.MemoryType.Preference => "01-Memory/Preferences",
                MemoryDomain.MemoryType.Knowledge => "04-Knowledge",
                MemoryDomain.MemoryType.Meeting => "08-Meetings",
                MemoryDomain.MemoryType.Agent => "05-Agents",
                MemoryDomain.MemoryType.Incident => "09-Incidents",
                MemoryDomain.MemoryType.Note => "00-Inbox",
                _ => "00-Inbox"
            };

            // Generate slug from title (could change if title changed)
            var slug = _slugGenerator.Generate(title);
            if (string.IsNullOrWhiteSpace(slug))
                slug = "memory";

            var relativePath = Path.Combine(folder, $"{slug}.md");

            var memoryFile = await _vaultRepository.UpdateAsync(updatedMemory, relativePath, cancellationToken);

            return new MemoryResponse
            {
                Id = memoryFile.Id,
                Title = memoryFile.Title,
                Type = memoryFile.Type.ToString(),
                Content = memoryFile.Content,
                Project = memoryFile.Project,
                Tags = memoryFile.Tags,
                Relations = memoryFile.Relations,
                Source = memoryFile.Source,
                Importance = memoryFile.Importance,
                CreatedAt = memoryFile.CreatedAt,
                UpdatedAt = memoryFile.UpdatedAt
            };
        }
    }
}
