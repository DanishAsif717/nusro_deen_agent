
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NusroDeen.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelId;

    public GeminiService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["Gemini:ApiKey"]
            ?? throw new Exception("Gemini API Key missing!");

        _modelId = configuration["Gemini:ModelId"]
            ?? throw new Exception("Gemini Model ID missing!");
    }

    public async Task<string> SendMessageAsync(string message)
    {
        var requestBody = new
        {
            model = _modelId,
            input = message
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://generativelanguage.googleapis.com/v1beta/interactions"
        );

        request.Headers.Add("x-goog-api-key", _apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        using var response = await _httpClient.SendAsync(request);

        Console.WriteLine("Response:");
        Console.WriteLine(response);

        var responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Response Content:");
        Console.WriteLine(responseContent);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(responseContent);
        Console.WriteLine("Document:");
        Console.WriteLine(document.RootElement.ToString());

        var steps = document.RootElement.GetProperty("steps");
        Console.WriteLine("Dev Steps:");
        Console.WriteLine(steps.ToString());

        foreach (var step in steps.EnumerateArray())
        {
            if (step.TryGetProperty("type", out var type) &&
                type.GetString() == "model_output" &&
                step.TryGetProperty("content", out var content))
            {
                foreach (var item in content.EnumerateArray())
                {
                    if (item.TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? string.Empty;
                    }
                }
            }
        }

        return "No response received from Gemini.";
    }
}
