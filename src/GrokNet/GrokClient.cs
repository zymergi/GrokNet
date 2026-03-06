using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GrokNet;

public partial class GrokClient(HttpClient httpClient, string apiKey)
{
    private const string BaseUrl = "https://api.x.ai/v1";

    internal readonly HttpClient _httpClient = ConfigureHttpClient(httpClient, apiKey);

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
