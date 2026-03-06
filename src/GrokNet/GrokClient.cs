using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GrokNet.ChatCompletions;
using GrokNet.Responses;
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

    public async Task<ChatCompletionResponse> ChatCompletionAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<ChatCompletionRequest, ChatCompletionResponse>(
            "/v1/chat/completions", request, cancellationToken);
    }

    public async Task<CreateResponseResponse> CreateResponseAsync(
        CreateResponseRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<CreateResponseRequest, CreateResponseResponse>(
            "/v1/responses", request, cancellationToken);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path, TRequest request, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions.Default);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(path, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(responseJson, JsonOptions.Default)!;
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
