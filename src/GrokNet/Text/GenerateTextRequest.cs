namespace GrokNet.Text;

public class GenerateTextRequest
{
    public required string Model { get; set; }
    public required List<Message> Input { get; set; }
}
