using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Types.Interfaces;

public interface IActionDialogue
{ 
    public IEnumerable<BotAction> Options { get; set; }
    public Dictionary<string, BotAction> Actions { get; }
    public void WrongAnswer(Context ctx);
    public string WrongAnswerText(Context ctx);
    public OptionsProvider GetOptions();
}