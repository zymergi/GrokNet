using System.Text.Json.Serialization;

namespace GrokNet;

// --- Chat Completion Request ---

public class ChatCompletionRequest
{
    public required string Model { get; set; }
    public required List<Message> Messages { get; set; }
    public double? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public double? TopP { get; set; }
    public bool? Stream { get; set; }
}

public class Message
{
    public required string Role { get; set; }
    public required string Content { get; set; }
}

// --- Chat Completion Response ---

public class ChatCompletionResponse
{
    public required string Id { get; set; }
    public required string Object { get; set; }
    public long Created { get; set; }
    public required string Model { get; set; }
    public required List<Choice> Choices { get; set; }
    public Usage? Usage { get; set; }
}

public class Choice
{
    public int Index { get; set; }
    public required Message Message { get; set; }
    public string? FinishReason { get; set; }
}

public class Usage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

// --- Models ---

public class ModelsResponse
{
    public required List<ModelInfo> Data { get; set; }
}

public class ModelInfo
{
    public required string Id { get; set; }
    public required string Object { get; set; }
    public long Created { get; set; }
    public required string OwnedBy { get; set; }
}
