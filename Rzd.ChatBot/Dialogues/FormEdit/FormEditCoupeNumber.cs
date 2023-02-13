using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormEditCoupeNumber : OptionOrInputDialogue
{
    public FormEditCoupeNumber() : base("formEditCoupeNumber")
    {
        Options = new BotAction[] {GoBack};
    }
    [OptionIndex(0)]
    private ValueTask<State> GoBack(Context ctx) => new(State.MyForm);

    public override State State => State.FormEditCoupeNumber;
    public override ValueTask<State> ProceedInput(Context ctx)
    {
        throw new NotImplementedException();
    }

    public override bool Validate(Message msg)
    {
        throw new NotImplementedException();
    }
}