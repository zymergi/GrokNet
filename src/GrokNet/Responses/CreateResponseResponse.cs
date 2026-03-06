using GrokNet.Common;

namespace GrokNet.Responses;

public class CreateResponseResponse : IGrokResult
{
    public string Id { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public long? CreatedAt { get; set; }
    public long? CompletedAt { get; set; }
    public string? Status { get; set; }
    public List<OutputItem> Output { get; set; } = [];
    public ResponseUsage? Usage { get; set; }
    public Reasoning? Reasoning { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public string? ToolChoice { get; set; }
    public bool? ParallelToolCalls { get; set; }
    public string? PreviousResponseId { get; set; }
    public string? ServiceTier { get; set; }
    public string? Truncation { get; set; }
    public double? PresencePenalty { get; set; }
    public double? FrequencyPenalty { get; set; }
    public bool? Store { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public bool? Background { get; set; }
    public string? Error { get; set; }
    public string? Instructions { get; set; }

    string? IGrokResult.Text => Output
        .SelectMany(o => o.Content)
        .FirstOrDefault(c => c.Type is "output_text")?.Text;

    TokenUsage? IGrokResult.Usage => Usage is null ? null : new TokenUsage
    {
        InputTokens = Usage.InputTokens,
        OutputTokens = Usage.OutputTokens,
        TotalTokens = Usage.TotalTokens,
        ReasoningTokens = Usage.OutputTokensDetails?.ReasoningTokens,
        CachedTokens = Usage.InputTokensDetails?.CachedTokens
    };
}
