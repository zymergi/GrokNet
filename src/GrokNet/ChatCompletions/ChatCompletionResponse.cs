using GrokNet.Common;

namespace GrokNet.ChatCompletions;

public class ChatCompletionResponse : IGrokResult
{
    public string Id { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
    public long Created { get; set; }
    public string Model { get; set; } = string.Empty;
    public List<Choice> Choices { get; set; } = [];
    public ChatCompletionUsage? Usage { get; set; }
    public string? SystemFingerprint { get; set; }

    string? IGrokResult.Text => Choices.Count > 0 ? Choices[0].Message?.Content : null;

    TokenUsage? IGrokResult.Usage => Usage is null ? null : new TokenUsage
    {
        InputTokens = Usage.PromptTokens,
        OutputTokens = Usage.CompletionTokens,
        TotalTokens = Usage.TotalTokens,
        ReasoningTokens = Usage.CompletionTokensDetails?.ReasoningTokens,
        CachedTokens = Usage.PromptTokensDetails?.CachedTokens
    };
}
