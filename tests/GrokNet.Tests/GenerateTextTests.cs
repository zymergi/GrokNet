using System.Net;
using System.Text.Json;
using GrokNet.ChatCompletions;
using GrokNet.Common;
using GrokNet.Responses;
using GrokNet.Text;

namespace GrokNet.Tests;

public class ChatCompletionTests
{
    private static readonly string ChatCompletionJson = """
        {
          "id": "a3d1008e-4544-40d4-d075-11527e794e4a",
          "object": "chat.completion",
          "created": 1752854522,
          "model": "grok-4-0709",
          "choices": [
            {
              "index": 0,
              "message": {
                "role": "assistant",
                "content": "101 multiplied by 3 is 303.",
                "refusal": null
              },
              "finish_reason": "stop"
            }
          ],
          "usage": {
            "prompt_tokens": 32,
            "completion_tokens": 9,
            "total_tokens": 135,
            "prompt_tokens_details": {
              "text_tokens": 32,
              "audio_tokens": 0,
              "image_tokens": 0,
              "cached_tokens": 6
            },
            "completion_tokens_details": {
              "reasoning_tokens": 94,
              "audio_tokens": 0,
              "accepted_prediction_tokens": 0,
              "rejected_prediction_tokens": 0
            },
            "num_sources_used": 0
          },
          "system_fingerprint": "fp_3a7881249c"
        }
        """;

    [Fact]
    public async Task ChatCompletionAsync_SendsCorrectRequest()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, ChatCompletionJson);
        var client = new GrokClient(new HttpClient(handler), "test-key");

        var response = await client.ChatCompletionAsync(new ChatCompletionRequest
        {
            Model = "grok-4",
            Messages = [Message.System("You are Grok."), Message.User("What is 101*3?")]
        });

        Assert.Equal("a3d1008e-4544-40d4-d075-11527e794e4a", response.Id);
        Assert.Equal("grok-4-0709", response.Model);
        Assert.Single(response.Choices);
        Assert.Equal("101 multiplied by 3 is 303.", response.Choices[0].Message!.Content);
        Assert.Equal("stop", response.Choices[0].FinishReason);
        Assert.Equal(32, response.Usage!.PromptTokens);
        Assert.Equal(94, response.Usage.CompletionTokensDetails!.ReasoningTokens);

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/v1/chat/completions", handler.LastRequest.RequestUri!.AbsolutePath);

        var body = JsonSerializer.Deserialize<JsonElement>(handler.LastRequestContent!);
        Assert.Equal("grok-4", body.GetProperty("model").GetString());
        Assert.Equal(2, body.GetProperty("messages").GetArrayLength());
    }

    [Fact]
    public async Task ChatCompletionAsync_WithReasoningEffort()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, ChatCompletionJson);
        var client = new GrokClient(new HttpClient(handler), "test-key");

        await client.ChatCompletionAsync(new ChatCompletionRequest
        {
            Model = "grok-3-mini",
            Messages = [Message.User("What is 101*3?")],
            ReasoningEffort = "high"
        });

        var body = JsonSerializer.Deserialize<JsonElement>(handler.LastRequestContent!);
        Assert.Equal("high", body.GetProperty("reasoning_effort").GetString());
    }

    [Fact]
    public async Task ChatCompletionAsync_DeferredRequest()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, ChatCompletionJson);
        var client = new GrokClient(new HttpClient(handler), "test-key");

        await client.ChatCompletionAsync(new ChatCompletionRequest
        {
            Model = "grok-4",
            Messages = [Message.User("hi")],
            Deferred = true
        });

        var body = JsonSerializer.Deserialize<JsonElement>(handler.LastRequestContent!);
        Assert.True(body.GetProperty("deferred").GetBoolean());
    }

    [Fact]
    public async Task ChatCompletionAsync_IGrokResult()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, ChatCompletionJson);
        var client = new GrokClient(new HttpClient(handler), "test-key");

        IGrokResult result = await client.ChatCompletionAsync(new ChatCompletionRequest
        {
            Model = "grok-4",
            Messages = [Message.User("hi")]
        });

        Assert.Equal("101 multiplied by 3 is 303.", result.Text);
        Assert.Equal(135, result.Usage!.TotalTokens);
        Assert.Equal(32, result.Usage.InputTokens);
        Assert.Equal(9, result.Usage.OutputTokens);
        Assert.Equal(94, result.Usage.ReasoningTokens);
        Assert.Equal(6, result.Usage.CachedTokens);
    }

    [Fact]
    public async Task ChatCompletionAsync_ThrowsOnError()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, "{}");
        var client = new GrokClient(new HttpClient(handler), "bad-key");

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.ChatCompletionAsync(new ChatCompletionRequest
            {
                Model = "grok-4",
                Messages = [Message.User("hi")]
            }));
    }
}

public class CreateResponseTests
{
    private static readonly string ResponseJson = """
        {
          "id": "57c5d7e0-7be7-a461-438a-fe1671c8d81e",
          "object": "response",
          "model": "grok-4-1-fast-reasoning",
          "created_at": 1771331237,
          "completed_at": 1771331241,
          "status": "completed",
          "output": [
            {
              "content": [
                {
                  "type": "output_text",
                  "text": "42 is the answer."
                }
              ],
              "id": "msg_57c5d7e0",
              "role": "assistant",
              "type": "message",
              "status": "completed"
            }
          ],
          "usage": {
            "input_tokens": 163,
            "output_tokens": 375,
            "total_tokens": 538,
            "input_tokens_details": { "cached_tokens": 151 },
            "output_tokens_details": { "reasoning_tokens": 190 }
          },
          "reasoning": { "effort": "medium", "summary": "detailed" },
          "temperature": 0.7,
          "top_p": 0.95
        }
        """;

    [Fact]
    public async Task CreateResponseAsync_SendsCorrectRequest()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, ResponseJson);
        var client = new GrokClient(new HttpClient(handler), "test-key");

        var response = await client.CreateResponseAsync(new CreateResponseRequest
        {
            Model = "grok-4-1-fast-reasoning",
            Input = [Message.User("What is the meaning of life?")]
        });

        Assert.Equal("57c5d7e0-7be7-a461-438a-fe1671c8d81e", response.Id);
        Assert.Equal("grok-4-1-fast-reasoning", response.Model);
        Assert.Equal("completed", response.Status);
        Assert.Single(response.Output);
        Assert.Equal("42 is the answer.", response.Output[0].Content[0].Text);
        Assert.Equal(190, response.Usage!.OutputTokensDetails!.ReasoningTokens);
        Assert.Equal("medium", response.Reasoning!.Effort);

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/v1/responses", handler.LastRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task CreateResponseAsync_WithReasoning()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, ResponseJson);
        var client = new GrokClient(new HttpClient(handler), "test-key");

        await client.CreateResponseAsync(new CreateResponseRequest
        {
            Model = "grok-3-mini",
            Input = [Message.User("hi")],
            Reasoning = new Reasoning { Effort = "high", Summary = "detailed" }
        });

        var body = JsonSerializer.Deserialize<JsonElement>(handler.LastRequestContent!);
        var reasoning = body.GetProperty("reasoning");
        Assert.Equal("high", reasoning.GetProperty("effort").GetString());
        Assert.Equal("detailed", reasoning.GetProperty("summary").GetString());
    }

    [Fact]
    public async Task CreateResponseAsync_IGrokResult()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, ResponseJson);
        var client = new GrokClient(new HttpClient(handler), "test-key");

        IGrokResult result = await client.CreateResponseAsync(new CreateResponseRequest
        {
            Model = "grok-4-1-fast-reasoning",
            Input = [Message.User("hi")]
        });

        Assert.Equal("42 is the answer.", result.Text);
        Assert.Equal(538, result.Usage!.TotalTokens);
        Assert.Equal(163, result.Usage.InputTokens);
        Assert.Equal(375, result.Usage.OutputTokens);
        Assert.Equal(190, result.Usage.ReasoningTokens);
        Assert.Equal(151, result.Usage.CachedTokens);
    }

    [Fact]
    public async Task CreateResponseAsync_ThrowsOnError()
    {
        var handler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, "{}");
        var client = new GrokClient(new HttpClient(handler), "bad-key");

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.CreateResponseAsync(new CreateResponseRequest
            {
                Model = "grok-4",
                Input = [Message.User("hi")]
            }));
    }
}
