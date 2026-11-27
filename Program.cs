
using Microsoft.Extensions.Configuration;
using NusroDeen.Services;

// ==========================================
// 1. Load User Secrets
// ==========================================

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// ==========================================
// 2. Initialize Gemini Service
// ==========================================

using var httpClient = new HttpClient();

var geminiService = new GeminiService(
    httpClient,
    configuration
);

// ==========================================
// 3. Send Message to Gemini
// ==========================================

try
{
    Console.WriteLine("\nNusroDeen is ready!");

    Console.Write("\nYou: ");
    string message = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(message))
    {
        Console.WriteLine("Please enter a message.");
    }
    else
    {
        Console.WriteLine("\nConnecting to Gemini...");

        string response = await geminiService.SendMessageAsync(message);

        Console.WriteLine("\nNusroDeen:");
        Console.WriteLine(response);
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine("\nHTTP Error:");
    Console.WriteLine(ex.Message);
}
catch (Exception ex)
{
    Console.WriteLine("\nUnexpected Error:");
    Console.WriteLine(ex.ToString());
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
