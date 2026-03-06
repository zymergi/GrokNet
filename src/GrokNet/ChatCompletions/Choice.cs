namespace GrokNet.ChatCompletions;

public class Choice
{
    public int Index { get; set; }
    public ChatMessage? Message { get; set; }
    public string? FinishReason { get; set; }
}
