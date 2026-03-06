using System.Net;
using System.Text.Json;

namespace GrokNet.Tests;

public class GrokClientTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private static GrokClient CreateClient(MockHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.x.ai/v1") };
        return new GrokClient(httpClient, "test-api-key");
    }

    [Fact]
    public async Task GenerateTextAsync_ReturnsValidResponse()
    {
        var rawResponse = new GenerateTextRawResponse
        {
            Id = "chatcmpl-123",
            Object = "chat.completion",
            Created = 1700000000,
            Model = "grok-3",
            Choices =
            [
                new GenerateTextChoice
                {
                    Index = 0,
                    Message = new Message { Role = "assistant", Content = "Hello!" },
                    FinishReason = "stop"
                }
            ],
            Usage = new Usage { PromptTokens = 10, CompletionTokens = 5, TotalTokens = 15 }
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(rawResponse, JsonOptions), HttpStatusCode.OK);
        var client = CreateClient(handler);

        var request = new GenerateTextRequest
        {
            Model = "grok-3",
            Messages = [new Message { Role = "user", Content = "Hi" }]
        };

        var result = await client.GenerateTextAsync(request);

        Assert.Equal("Hello!", result.Text);
        Assert.Equal("chatcmpl-123", result.RawResponse.Id);
        Assert.NotNull(result.RawResponse.Usage);
        Assert.Equal(15, result.RawResponse.Usage.TotalTokens);
    }

    [Fact]
    public async Task GenerateTextAsync_ThrowsOnErrorStatus()
    {
        var handler = new MockHttpMessageHandler("{\"error\":\"bad request\"}", HttpStatusCode.BadRequest);
        var client = CreateClient(handler);

        var request = new GenerateTextRequest
        {
            Model = "grok-3",
            Messages = [new Message { Role = "user", Content = "Hi" }]
        };

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GenerateTextAsync(request));
    }

    [Fact]
    public async Task ListModelsAsync_ReturnsModels()
    {
        var expected = new ModelsResponse
        {
            Data =
            [
                new ModelInfo { Id = "grok-3", Object = "model", Created = 1700000000, OwnedBy = "xai" }
            ]
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(expected, JsonOptions), HttpStatusCode.OK);
        var client = CreateClient(handler);

        var result = await client.ListModelsAsync();

        Assert.Single(result.Data);
        Assert.Equal("grok-3", result.Data[0].Id);
    }

    [Fact]
    public async Task GenerateTextAsync_SendsAuthorizationHeader()
    {
        var handler = new MockHttpMessageHandler("{\"id\":\"1\",\"object\":\"chat.completion\",\"created\":0,\"model\":\"grok-3\",\"choices\":[{\"index\":0,\"message\":{\"role\":\"assistant\",\"content\":\"hi\"},\"finish_reason\":\"stop\"}]}", HttpStatusCode.OK);
        var client = CreateClient(handler);

        var request = new GenerateTextRequest
        {
            Model = "grok-3",
            Messages = [new Message { Role = "user", Content = "Hi" }]
        };

        await client.GenerateTextAsync(request);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("Bearer", handler.LastRequest!.Headers.Authorization?.Scheme);
        Assert.Equal("test-api-key", handler.LastRequest.Headers.Authorization?.Parameter);
    }
}
