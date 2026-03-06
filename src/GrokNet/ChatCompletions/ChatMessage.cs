namespace GrokNet.ChatCompletions;

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Refusal { get; set; }
}
