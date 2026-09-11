using System;
using System.Text.RegularExpressions;

namespace Aurora.Infrastructure.Knowledge
{
    public interface ISlugGenerator
    {
        string Generate(string value);
    }

    public class SlugGenerator : ISlugGenerator
    {
        public string Generate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "memory";

            // Normalize: remove accents? We'll just lower case and replace non-alphanumeric with hyphens
            var normalized = value.ToLowerInvariant();
            // Replace any non-letter or digit with hyphen
            normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
            // Replace whitespace and underscores with hyphen
            normalized = Regex.Replace(normalized, @"[\s_-]+", "-");
            // Trim hyphens from start/end
            normalized = normalized.Trim('-');
            // Collapse multiple hyphens already done
            // Limit length to 100 chars
            if (normalized.Length > 100)
                normalized = normalized.Substring(0, 100);
            return normalized;
        }
    }
}
