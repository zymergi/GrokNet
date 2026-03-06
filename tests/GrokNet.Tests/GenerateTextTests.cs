using System.Net;
using System.Text.Json;
using GrokNet.Text;

namespace GrokNet.Tests;

public class GenerateTextTests
{
    [Fact]
    public async Task GenerateTextAsync_SendsCorrectRequest()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK,
            """{"id": "resp-123", "output_text": "42"}""");
        var client = new GrokClient(new HttpClient(handler), "test-key");

        var messages = new List<Message>
        {
            Message.System("You are Grok."),
            Message.User("What is the meaning of life?")
        };

        var response = await client.GenerateTextAsync("grok-4-1-fast-reasoning", messages);

        Assert.Equal("resp-123", response.Id);
        Assert.Equal("42", response.OutputText);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Equal("/v1/responses", handler.LastRequest.RequestUri!.AbsolutePath);

        var body = JsonSerializer.Deserialize<JsonElement>(handler.LastRequestContent!);
        Assert.Equal("grok-4-1-fast-reasoning", body.GetProperty("model").GetString());
        Assert.Equal(2, body.GetProperty("input").GetArrayLength());
    }

    [Fact]
    public async Task GenerateTextAsync_ThrowsOnError()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, "{}");
        var client = new GrokClient(new HttpClient(handler), "bad-key");

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.GenerateTextAsync("grok-4", [Message.User("hi")]));
    }
}
