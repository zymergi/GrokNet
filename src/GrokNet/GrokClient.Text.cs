using System.Net.Http.Json;

namespace GrokNet;

public partial class GrokClient
{
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
}
