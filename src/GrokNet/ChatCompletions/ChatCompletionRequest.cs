using GrokNet.Text;

namespace GrokNet.ChatCompletions;

public class ChatCompletionRequest
{
    public required string Model { get; set; }
    public required List<Message> Messages { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? MaxTokens { get; set; }
    public bool? Stream { get; set; }
    public string? ReasoningEffort { get; set; }
    public int? N { get; set; }
    public double? FrequencyPenalty { get; set; }
    public double? PresencePenalty { get; set; }
    public List<string>? Stop { get; set; }
    public string? User { get; set; }
    public bool? Deferred { get; set; }
}
