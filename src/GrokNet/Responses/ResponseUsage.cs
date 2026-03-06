namespace GrokNet.Responses;

public class ResponseUsage
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
    public InputTokensDetails? InputTokensDetails { get; set; }
    public OutputTokensDetails? OutputTokensDetails { get; set; }
    public int? NumSourcesUsed { get; set; }
    public int? NumServerSideToolsUsed { get; set; }
}

public class InputTokensDetails
{
    public int? CachedTokens { get; set; }
}

public class OutputTokensDetails
{
    public int? ReasoningTokens { get; set; }
}
