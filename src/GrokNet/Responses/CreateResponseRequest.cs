using GrokNet.Text;

namespace GrokNet.Responses;

public class CreateResponseRequest
{
    public required string Model { get; set; }
    public required List<Message> Input { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? MaxOutputTokens { get; set; }
    public bool? Stream { get; set; }
    public Reasoning? Reasoning { get; set; }
    public double? FrequencyPenalty { get; set; }
    public double? PresencePenalty { get; set; }
    public string? User { get; set; }
    public bool? Store { get; set; }
    public string? Instructions { get; set; }
    public string? PreviousResponseId { get; set; }
    public bool? Background { get; set; }
    public string? ServiceTier { get; set; }
    public string? Truncation { get; set; }
}

public class Reasoning
{
    public string? Effort { get; set; }
    public string? Summary { get; set; }
}
