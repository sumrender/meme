using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Backend.Services
{
    public class CloudflareAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public CloudflareAiService(HttpClient httpClient, IConfiguration config)
        {
            var baseUrl = config.GetValue<string>("CloudflareSettings:BaseUrl");
            var apiToken = config.GetValue<string>("CloudflareSettings:ApiToken");
            _model = config.GetValue<string>("CloudflareSettings:Model") ?? "@cf/meta/llama-3.1-8b-instruct";

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("CloudflareSettings:BaseUrl is not configured.");
            }

            if (string.IsNullOrEmpty(apiToken))
            {
                throw new ArgumentException("CloudflareSettings:ApiToken is not configured.");
            }

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiToken);
        }

        public async Task<string> AskAsync(string input, CancellationToken cancellationToken = default)
        {
            var body = new
            {
                model = _model,
                messages = new object[]
                {
                    new { role = "user", content = input }
                },
                temperature = 0.7,
                stop = new[] { "\nHuman:", "\nAI Assistant:", "\nUser:", "\nAI:" }
            };

            var response = await _httpClient.PostAsJsonAsync("chat/completions", body, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(
                    $"Cloudflare API error ({(int)response.StatusCode} {response.StatusCode}): {errorBody}");
            }

            using var doc = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken);

            if (doc is null)
            {
                return "No content returned by the API.";
            }

            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                throw new InvalidOperationException(
                    $"Unexpected response shape from Cloudflare: {doc.RootElement}");
            }

            var contentElement = choices[0]
                .GetProperty("message")
                .GetProperty("content");

            string? content = contentElement.ValueKind switch
            {
                JsonValueKind.String => contentElement.GetString(),
                JsonValueKind.Object or JsonValueKind.Array => contentElement.GetRawText(),
                _ => contentElement.ToString()
            };

            return content?.Trim() ?? "No content returned by the API.";
        }
    }
}