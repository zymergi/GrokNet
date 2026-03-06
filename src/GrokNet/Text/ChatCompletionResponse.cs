namespace GrokNet;

public class ChatCompletionResponse
{
    public required string Id { get; set; }
    public required string Object { get; set; }
    public long Created { get; set; }
    public required string Model { get; set; }
    public required List<Choice> Choices { get; set; }
    public Usage? Usage { get; set; }
}
