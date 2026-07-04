using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Google.GenAI;
using Google.GenAI.Types;

namespace Backend.Services
{
    public class GeminiService : IAiService
    {
        private readonly Client _client;
        private readonly string _model;

        public GeminiService(IConfiguration config)
        {
            var apiKey = config.GetValue<string>("GeminiSettings:ApiKey");
            _model = config.GetValue<string>("GeminiSettings:Model") ?? "gemini-1.5-flash";
            var apiEndpoint = config.GetValue<string>("GeminiSettings:ApiEndpoint");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("GeminiSettings:ApiKey is not configured properly in appsettings or environment variables.");
            }

            if (!string.IsNullOrEmpty(apiEndpoint))
            {
                var httpOptions = new HttpOptions
                {
                    BaseUrl = apiEndpoint
                };
                _client = new Client(apiKey: apiKey, httpOptions: httpOptions);
            }
            else
            {
                _client = new Client(apiKey: apiKey);
            }
        }

        public async Task<string> AskAsync(string input, CancellationToken cancellationToken = default)
        {
            var response = await _client.Models.GenerateContentAsync(
                model: _model,
                contents: input,
                cancellationToken: cancellationToken
            );

            return response?.Text ?? "No content returned by the API.";
        }

    }
}
