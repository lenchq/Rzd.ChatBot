namespace Rzd.ChatBot.Types;

public class InlineButtonsProvider
{
    public IEnumerable<IEnumerable<(string Key, string Text)>> Buttons { get; set; }
}