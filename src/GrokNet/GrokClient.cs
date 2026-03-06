using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GrokNet;

public class GrokClient(HttpClient httpClient, string apiKey)
{
    private const string BaseUrl = "https://api.x.ai/v1";

    private readonly HttpClient _httpClient = ConfigureHttpClient(httpClient, apiKey);

    private static HttpClient ConfigureHttpClient(HttpClient client, string apiKey)
    {
        client.BaseAddress ??= new Uri(BaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        return client;
    }

    public async Task<ChatCompletionResponse> ChatCompletionAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/v1/chat/completions",
            request,
            JsonOptions.Default,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(
            JsonOptions.Default, cancellationToken)
            ?? throw new InvalidOperationException("Received null response from API.");
    }

    public async Task<ModelsResponse> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/v1/models", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ModelsResponse>(
            JsonOptions.Default, cancellationToken)
            ?? throw new InvalidOperationException("Received null response from API.");
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
