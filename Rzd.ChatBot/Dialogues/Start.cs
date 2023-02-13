using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class StartDialogue : ActionDialogue
{
    public override State State => State.Starting;
    
    public StartDialogue()
        : base("start")
    {
        Options = new BotAction[] {OkAction};
    }

    [OptionIndex(0)]
    private ValueTask<State> OkAction(Context ctx)
        => new ValueTask<State>(State.GenderInput);
}