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
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aurora.Infrastructure.Knowledge
{
    public class VaultRepository : IVaultRepository
    {
        private readonly VaultOptions _options;
        private readonly IMarkdownGenerator _markdownGenerator;
        private readonly ISlugGenerator _slugGenerator;
        private static readonly Deserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        public VaultRepository(IOptions<VaultOptions> options, IMarkdownGenerator markdownGenerator, ISlugGenerator slugGenerator)
        {
            _options = options.Value;
            _markdownGenerator = markdownGenerator;
            _slugGenerator = slugGenerator;
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

        public async Task<MemoryFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            foreach (var filePath in Directory.EnumerateFiles(_options.VaultPath, "*.md", SearchOption.AllDirectories))
            {
                try
                {
                    var text = await File.ReadAllTextAsync(filePath, cancellationToken);
                    var memory = ParseMemoryFromMarkdown(text, filePath);
                    if (memory != null && memory.Id == id)
                    {
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
                            FilePath = filePath,
                            RelativePath = Path.GetRelativePath(_options.VaultPath, filePath)
                        };
                    }
                }
                catch (Exception ex)
                {
                    // Log error and continue
                    continue;
                }
            }
            return null;
        }

        public async Task<MemoryFile> UpdateAsync(Memory memory, string relativePath, CancellationToken cancellationToken = default)
        {
            if (memory == null) throw new ArgumentNullException(nameof(memory));
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("relativePath is required");

            var fullPath = Path.Combine(_options.VaultPath, relativePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Memory file not found: {relativePath}");
            }

            // Security check: ensure path is within vault
            var vaultFullPath = Path.GetFullPath(_options.VaultPath);
            var fullPathAbsolute = Path.GetFullPath(fullPath);
            if (!fullPathAbsolute.StartsWith(vaultFullPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Path traversal attempt detected.");
            }

            // Read existing to preserve CreatedAt if not set? We'll keep the memory's CreatedAt as is (should be set).
            // Generate markdown
            var markdown = _markdownGenerator.Generate(memory);

            // Write atomically
            var tempPath = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath) + ".tmp");
            await File.WriteAllTextAsync(tempPath, markdown, Encoding.UTF8, cancellationToken);
            File.Move(tempPath, fullPath, true);

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
                FilePath = fullPath,
                RelativePath = relativePath
            };
        }

        public async Task<IReadOnlyCollection<MemoryFile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var files = new List<MemoryFile>();
            foreach (var filePath in Directory.EnumerateFiles(_options.VaultPath, "*.md", SearchOption.AllDirectories))
            {
                try
                {
                    var text = await File.ReadAllTextAsync(filePath, cancellationToken);
                    var memory = ParseMemoryFromMarkdown(text, filePath);
                    if (memory != null)
                    {
                        files.Add(new MemoryFile
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
                            FilePath = filePath,
                            RelativePath = Path.GetRelativePath(_options.VaultPath, filePath)
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Skip file on error
                    continue;
                }
            }
            return files;
        }

        private MemoryDomain.Memory? ParseMemoryFromMarkdown(string markdown, string filePath)
        {
            try
            {
                // Split frontmatter
                var parts = markdown.Split(new[] { "
---
", "
---
" }, StringSplitOptions.None);
                if (parts.Length < 3)
                {
                    // No frontmatter
                    return null;
                }
                var frontmatter = parts[1];
                var rest = parts[2];

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();
                var frontmatterObj = deserializer.Deserialize<FrontmatterData>(frontmatter);

                // Parse content: first line is title (starting with # ), then blank line, then content, then optional relations section.
                var lines = rest.Split(new[] { "
", "
" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    return null;
                }
                // Assume first line is title with # 
                var titleLine = lines[0];
                string title = titleLine.TrimStart();
                if (title.StartsWith("# "))
                {
                    title = title.Substring(2).Trim();
                }
                else
                {
                    title = titleLine.Trim();
                }

                // Find where content ends and relations start
                int contentEnd = lines.Length;
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].TrimStart().StartsWith("## Relacionamentos"))
                    {
                        contentEnd = i;
                        break;
                    }
                }
                var contentLines = lines.Skip(1).Take(contentEnd - 1).ToArray();
                var content = string.Join("
", contentLines).TrimEnd();

                // Extract relations from the rest (if any)
                IReadOnlyCollection<string>? relations = null;
                if (contentEnd < lines.Length)
                {
                    var relationLines = lines.Skip(contentEnd + 1).ToArray(); // skip the header line
                    var rels = new List<string>();
                    foreach (var rel in relationLines)
                    {
                        var trimmed = rel.Trim();
                        if (trimmed.StartsWith("- "))
                        {
                            trimmed = trimmed.Substring(2).Trim();
                        }
                        // Remove wikilinks brackets if present
                        if (trimmed.StartsWith("[[") && trimmed.EndsWith("]]"))
                        {
                            trimmed = trimmed.Substring(2, trimmed.Length - 4);
                        }
                        if (!string.IsNullOrWhiteSpace(trimmed))
                        {
                            rels.Add(trimmed);
                        }
                    }
                    if (rels.Any())
                    {
                        relations = rels;
                    }
                }

                // Build Memory object
                var memory = new MemoryDomain.Memory(
                    frontmatterObj.Id,
                    title,
                    frontmatterObj.Type,
                    content,
                    frontmatterObj.Project,
                    frontmatterObj.Tags,
                    relations,
                    frontmatterObj.Source,
                    frontmatterObj.Importance,
                    frontmatterObj.CreatedAt,
                    frontmatterObj.UpdatedAt
                );

                return memory;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private class FrontmatterData
        {
            public Guid Id { get; set; }
            public string Type { get; set; } = string.Empty;
            public string? Project { get; set; }
            public IReadOnlyCollection<string>? Tags { get; set; }
            public IReadOnlyCollection<string>? Relations { get; set; }
            public string? Source { get; set; }
            public decimal Importance { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
    }
}
