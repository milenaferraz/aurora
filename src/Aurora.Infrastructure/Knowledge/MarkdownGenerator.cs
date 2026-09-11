using System;
using System.Text;
using Aurora.Domain.Knowledge;

namespace Aurora.Infrastructure.Knowledge
{
    public interface IMarkdownGenerator
    {
        string Generate(Memory memory);
    }

    public class MarkdownGenerator : IMarkdownGenerator
    {
        public string Generate(Memory memory)
        {
            var sb = new StringBuilder();
            sb.AppendLine("---");
            sb.AppendLine($"id: {memory.Id}");
            sb.AppendLine($"type: {memory.Type.ToString().ToLowerInvariant()}");
            if (!string.IsNullOrWhiteSpace(memory.Project))
                sb.AppendLine($"project: {memory.Project}");
            if (!string.IsNullOrWhiteSpace(memory.Source))
                sb.AppendLine($"source: {memory.Source}");
            sb.AppendLine($"importance: {memory.Importance:F2}");
            sb.AppendLine($"createdAt: {memory.CreatedAt:O}");
            sb.AppendLine($"updatedAt: {memory.UpdatedAt:O}");
            if (memory.Tags != null && memory.Tags.Any())
            {
                sb.AppendLine("tags:");
                foreach (var tag in memory.Tags)
                {
                    sb.AppendLine($"  - {tag}");
                }
            }
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine($"# {memory.Title}");
            sb.AppendLine();
            sb.AppendLine(memory.Content);
            sb.AppendLine();
            if (memory.Relations != null && memory.Relations.Any())
            {
                sb.AppendLine("## Relacionamentos");
                sb.AppendLine();
                foreach (var relation in memory.Relations)
                {
                    // Ensure wikilink format
                    var clean = relation.Trim();
                    if (clean.StartsWith("[[") && clean.EndsWith("]]"))
                    {
                        sb.AppendLine($"- {clean}");
                    }
                    else
                    {
                        sb.AppendLine($"- [[{clean}]]");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
