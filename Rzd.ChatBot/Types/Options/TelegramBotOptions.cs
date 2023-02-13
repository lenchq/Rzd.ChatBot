namespace Rzd.ChatBot.Types.Options;

public record TelegramBotOptions
{
    public string Token { get; set; } = string.Empty;
    public string[] Updates { get; set; } = Array.Empty<string>();
}