namespace GrokNet.Common;

public interface IGrokResult
{
    string Id { get; }
    string Model { get; }
    string? Text { get; }
    TokenUsage? Usage { get; }
}

public class TokenUsage
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
    public int? ReasoningTokens { get; set; }
    public int? CachedTokens { get; set; }
}
