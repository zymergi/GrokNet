using System.Net.Http.Json;

namespace GrokNet;

public partial class GrokClient
{
    public async Task<ModelsResponse> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/v1/models", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ModelsResponse>(
            JsonOptions.Default, cancellationToken)
            ?? throw new InvalidOperationException("Received null response from API.");
    }
}
