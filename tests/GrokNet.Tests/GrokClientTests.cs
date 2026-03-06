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
    public async Task ChatCompletionAsync_ReturnsValidResponse()
    {
        var expected = new ChatCompletionResponse
        {
            Id = "chatcmpl-123",
            Object = "chat.completion",
            Created = 1700000000,
            Model = "grok-3",
            Choices =
            [
                new Choice
                {
                    Index = 0,
                    Message = new Message { Role = "assistant", Content = "Hello!" },
                    FinishReason = "stop"
                }
            ],
            Usage = new Usage { PromptTokens = 10, CompletionTokens = 5, TotalTokens = 15 }
        };

        var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(expected, JsonOptions), HttpStatusCode.OK);
        var client = CreateClient(handler);

        var request = new ChatCompletionRequest
        {
            Model = "grok-3",
            Messages = [new Message { Role = "user", Content = "Hi" }]
        };

        var result = await client.ChatCompletionAsync(request);

        Assert.Equal("chatcmpl-123", result.Id);
        Assert.Single(result.Choices);
        Assert.Equal("Hello!", result.Choices[0].Message.Content);
        Assert.NotNull(result.Usage);
        Assert.Equal(15, result.Usage.TotalTokens);
    }

    [Fact]
    public async Task ChatCompletionAsync_ThrowsOnErrorStatus()
    {
        var handler = new MockHttpMessageHandler("{\"error\":\"bad request\"}", HttpStatusCode.BadRequest);
        var client = CreateClient(handler);

        var request = new ChatCompletionRequest
        {
            Model = "grok-3",
            Messages = [new Message { Role = "user", Content = "Hi" }]
        };

        await Assert.ThrowsAsync<HttpRequestException>(() => client.ChatCompletionAsync(request));
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
    public async Task ChatCompletionAsync_SendsAuthorizationHeader()
    {
        var handler = new MockHttpMessageHandler("{\"id\":\"1\",\"object\":\"chat.completion\",\"created\":0,\"model\":\"grok-3\",\"choices\":[]}", HttpStatusCode.OK);
        var client = CreateClient(handler);

        var request = new ChatCompletionRequest
        {
            Model = "grok-3",
            Messages = [new Message { Role = "user", Content = "Hi" }]
        };

        await client.ChatCompletionAsync(request);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("Bearer", handler.LastRequest!.Headers.Authorization?.Scheme);
        Assert.Equal("test-api-key", handler.LastRequest.Headers.Authorization?.Parameter);
    }
}
