namespace GrokNet;

public class Choice
{
    public int Index { get; set; }
    public required Message Message { get; set; }
    public string? FinishReason { get; set; }
}
