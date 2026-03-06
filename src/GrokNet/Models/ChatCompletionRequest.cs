namespace GrokNet;

public class ChatCompletionRequest
{
    public required string Model { get; set; }
    public required List<Message> Messages { get; set; }
    public double? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public double? TopP { get; set; }
    public bool? Stream { get; set; }
}
