using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Aurora.Application.Interfaces;

namespace Aurora.Infrastructure.Email
{
    public class EmailProvider : IEmailProvider
    {
        private readonly string _dataFilePath;

        public EmailProvider(IConfiguration configuration)
        {
            var filePath = configuration["EmailDataFilePath"];
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = Path.Combine(AppContext.BaseDirectory, "Data", "emails.json");
            }
            _dataFilePath = filePath;
        }

        public Task<EmailSummary> GetImportantCountAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return Task.FromResult(new EmailSummary(0));
                }

                var json = File.ReadAllText(_dataFilePath);
                var data = JsonSerializer.Deserialize<EmailData>(json);
                int count = data?.ImportantCount ?? 0;
                return Task.FromResult(new EmailSummary(count));
            }
            catch
            {
                return Task.FromResult(new EmailSummary(0));
            }
        }
    }

    public class EmailData
    {
        public int ImportantCount { get; set; }
    }
}
