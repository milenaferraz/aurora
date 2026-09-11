using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Aurora.Application.Interfaces;

namespace Aurora.Infrastructure.Memory
{
    public class MemoryProvider : IMemoryProvider
    {
        private readonly string _dataFilePath;
        private readonly ConcurrentDictionary<string, string> _store;
        private readonly object _lock = new();

        public MemoryProvider(IConfiguration configuration)
        {
            var filePath = configuration["MemoryDataFilePath"];
            if (string.IsNullOrWhiteSpace(filePath))
            {
                // Default to a file in the app directory
                filePath = Path.Combine(AppContext.BaseDirectory, "Data", "memory.json");
            }
            _dataFilePath = filePath;
            _store = new ConcurrentDictionary<string, string>();
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    var json = File.ReadAllText(_dataFilePath);
                    var data = JsonSerializer.Deserialize<ConcurrentDictionary<string, string>>(json);
                    if (data != null)
                    {
                        foreach (var kvp in data)
                        {
                            _store[kvp.Key] = kvp.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In real app, log error
                // For simplicity, start with empty store
            }
        }

        private void SaveToFile()
        {
            try
            {
                // Ensure directory exists
                var dir = Path.GetDirectoryName(_dataFilePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var json = JsonSerializer.Serialize(_store, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                // Log error
            }
        }

        public Task<string> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            _store.TryGetValue(key, out var value);
            return Task.FromResult(value);
        }

        public Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            _store[key] = value;
            SaveToFile();
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _store.TryRemove(key, out _);
            SaveToFile();
            return Task.CompletedTask;
        }
    }
}
