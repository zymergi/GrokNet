namespace GrokNet.Text;

public record Message(string Role, string Content)
{
    public static Message System(string content) => new("system", content);
    public static Message User(string content) => new("user", content);
    public static Message Assistant(string content) => new("assistant", content);
}
