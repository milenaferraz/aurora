using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Domain.Knowledge;
using Microsoft.Extensions.Options;

namespace Aurora.Infrastructure.Knowledge
{
    public class VaultRepository : IVaultRepository
    {
        private readonly VaultOptions _options;
        private readonly IMarkdownGenerator _markdownGenerator;

        public VaultRepository(IOptions<VaultOptions> options, IMarkdownGenerator markdownGenerator)
        {
            _options = options.Value;
            _markdownGenerator = markdownGenerator;
            // Ensure vault path exists
            if (!Directory.Exists(_options.VaultPath))
            {
                Directory.CreateDirectory(_options.VaultPath);
            }
        }

        public async Task<MemoryFile> CreateAsync(Memory memory, string relativePath, CancellationToken cancellationToken = default)
        {
            if (memory == null) throw new ArgumentNullException(nameof(memory));
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("relativePath is required");

            // Combine with vault path
            var fullPath = Path.Combine(_options.VaultPath, relativePath);
            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Security check: ensure path is within vault
            var vaultFullPath = Path.GetFullPath(_options.VaultPath);
            var fullPathAbsolute = Path.GetFullPath(fullPath);
            if (!fullPathAbsolute.StartsWith(vaultFullPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Path traversal attempt detected.");
            }

            // Check for collision: if file exists, we will create a new file with suffix -2, -3, etc.
            var finalPath = fullPath;
            if (File.Exists(finalPath))
            {
                var extension = Path.GetExtension(finalPath);
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(finalPath);
                var counter = 2;
                while (true)
                {
                    var candidate = Path.Combine(Path.GetDirectoryName(finalPath), $"{fileNameWithoutExt}-{counter}{extension}");
                    if (!File.Exists(candidate))
                    {
                        finalPath = candidate;
                        break;
                    }
                    counter++;
                }
            }

            // Generate markdown
            var markdown = _markdownGenerator.Generate(memory);

            // Write atomically: write to temp file then move
            var tempPath = Path.Combine(Path.GetDirectoryName(finalPath), Path.GetFileNameWithoutExtension(finalPath) + ".tmp");
            await File.WriteAllTextAsync(tempPath, markdown, Encoding.UTF8, cancellationToken);
            // Move/Replace
            File.Move(tempPath, finalPath, true);

            // Return memory file metadata (we could read back, but for simplicity we return what we have)
            return new MemoryFile
            {
                Id = memory.Id,
                Title = memory.Title,
                Type = memory.Type,
                Content = memory.Content,
                Project = memory.Project,
                Tags = memory.Tags,
                Relations = memory.Relations,
                Source = memory.Source,
                Importance = memory.Importance,
                CreatedAt = memory.CreatedAt,
                UpdatedAt = memory.UpdatedAt,
                FilePath = finalPath,
                RelativePath = Path.GetRelativePath(_options.VaultPath, finalPath)
            };
        }

        public Task<MemoryFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // For simplicity, we could scan all files, but we'll skip for now.
            return Task.FromResult<MemoryFile?>(null);
        }

        public Task<MemoryFile> UpdateAsync(Memory memory, string relativePath, CancellationToken cancellationToken = default)
        {
            // For simplicity, we'll just overwrite the file at relativePath (assuming it's the correct one)
            var fullPath = Path.Combine(_options.VaultPath, relativePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Memory file not found: {relativePath}");
            }
            var markdown = _markdownGenerator.Generate(memory);
            var tempPath = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath) + ".tmp");
            File.WriteAllText(tempPath, markdown, Encoding.UTF8);
            File.Move(tempPath, fullPath, true);
            return Task.FromResult(new MemoryFile
            {
                Id = memory.Id,
                Title = memory.Title,
                Type = memory.Type,
                Content = memory.Content,
                Project = memory.Project,
                Tags = memory.Tags,
                Relations = memory.Relations,
                Source = memory.Source,
                Importance = memory.Importance,
                CreatedAt = memory.CreatedAt,
                UpdatedAt = memory.UpdatedAt,
                FilePath = fullPath,
                RelativePath = relativePath
            });
        }

        public Task<IReadOnlyCollection<MemoryFile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var files = new List<MemoryFile>();
            foreach (var filePath in Directory.EnumerateFiles(_options.VaultPath, "*.md", SearchOption.AllDirectories))
            {
                // We could parse frontmatter, but for now we'll skip.
                files.Add(new MemoryFile
                {
                    FilePath = filePath,
                    RelativePath = Path.GetRelativePath(_options.VaultPath, filePath)
                });
            }
            return Task.FromResult<IReadOnlyCollection<MemoryFile>>(files);
        }
    }

    public class MemoryFile
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public Domain.Knowledge.MemoryType Type { get; set; }
        public string Content { get; set; } = default!;
        public string? Project { get; set; }
        public IReadOnlyCollection<string>? Tags { get; set; }
        public IReadOnlyCollection<string>? Relations { get; set; }
        public string? Source { get; set; }
        public decimal Importance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FilePath { get; set; } = default!;
        public string RelativePath { get; set; } = default!;
    }
}
