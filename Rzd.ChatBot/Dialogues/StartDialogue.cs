using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
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


    public override void WrongAnswer(Context ctx)
    {
        //TODO
    }

    public State OkAction(Context ctx)
        => State.SelectScanType;
}