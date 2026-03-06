namespace GrokNet.Responses;

public class OutputItem
{
    public string Id { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Status { get; set; }
    public List<OutputContent> Content { get; set; } = [];
}

public class OutputContent
{
    public string Type { get; set; } = string.Empty;
    public string? Text { get; set; }
    public List<object>? Logprobs { get; set; }
    public List<object>? Annotations { get; set; }
}
