namespace GrokNet.ChatCompletions;

public class ChatCompletionUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public PromptTokensDetails? PromptTokensDetails { get; set; }
    public CompletionTokensDetails? CompletionTokensDetails { get; set; }
    public int? NumSourcesUsed { get; set; }
}

public class PromptTokensDetails
{
    public int? TextTokens { get; set; }
    public int? AudioTokens { get; set; }
    public int? ImageTokens { get; set; }
    public int? CachedTokens { get; set; }
}

public class CompletionTokensDetails
{
    public int? ReasoningTokens { get; set; }
    public int? AudioTokens { get; set; }
    public int? AcceptedPredictionTokens { get; set; }
    public int? RejectedPredictionTokens { get; set; }
}
