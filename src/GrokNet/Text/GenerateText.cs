namespace GrokNet;

public class GenerateTextRequest
{
    public required string Model { get; set; }
    public required List<Message> Messages { get; set; }
    public double? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public double? TopP { get; set; }
}

public class GenerateTextResponse
{
    public required string Text { get; set; }
    public required GenerateTextRawResponse RawResponse { get; set; }
}

public class GenerateTextRawResponse
{
    public required string Id { get; set; }
    public required string Object { get; set; }
    public long Created { get; set; }
    public required string Model { get; set; }
    public required List<GenerateTextChoice> Choices { get; set; }
    public Usage? Usage { get; set; }
}

public class GenerateTextChoice
{
    public int Index { get; set; }
    public required Message Message { get; set; }
    public string? FinishReason { get; set; }
}
