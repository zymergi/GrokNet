using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GrokNet.Text;

namespace GrokNet;

/// <summary>
/// Client for interacting with the Grok API.
/// </summary>
/// <param name="httpClient">The HTTP client used for making requests.</param>
/// <param name="apiKey">The API key for authenticating with the Grok API.</param>
public class GrokClient(HttpClient httpClient, string apiKey)
{
    private const string BaseUrl = "https://api.x.ai/v1";

    internal readonly HttpClient _httpClient = ConfigureHttpClient(httpClient, apiKey);

    public async Task<GenerateTextResponse> GenerateTextAsync(
        string model,
        List<Message> input,
        CancellationToken cancellationToken = default)
    {
        var request = new GenerateTextRequest { Model = model, Input = input };
        var json = JsonSerializer.Serialize(request, JsonOptions.Default);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/responses", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GenerateTextResponse>(responseJson, JsonOptions.Default)!;
    }

    private static HttpClient ConfigureHttpClient(HttpClient client, string apiKey)
    {
        client.BaseAddress ??= new Uri(BaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        return client;
    }
}

internal static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
