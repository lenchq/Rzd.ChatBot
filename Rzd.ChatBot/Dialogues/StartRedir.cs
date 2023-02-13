using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class StartRedirDialogue : ActionDialogue
{
    public StartRedirDialogue() : base("start")
    {
        Options = new BotAction[] {Ok};
    }

    [OptionIndex(0)]
    private ValueTask<State> Ok(Context _) => new(State.Menu);

    public override State State => State.StartingRedir;
}