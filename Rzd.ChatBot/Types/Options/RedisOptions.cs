namespace Rzd.ChatBot.Types.Options;

public record RedisOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = default;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}