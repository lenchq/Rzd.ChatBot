using System.Globalization;
using System.Reflection.Emit;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class EditGenderDialogue : ActionDialogue
{
    public override State State => State.GenderInput;

    public EditGenderDialogue() : base("editGender")
    {
        Options = new BotAction[] {Male, Female};
    }

    [OptionIndex(0)]
    ValueTask<State> Male(Context ctx)
    {
        ctx.UserForm.Gender = Gender.Male;
        return new ValueTask<State>(State.NameInput);
    }

    [OptionIndex(1)]
    ValueTask<State> Female(Context ctx)
    {
        ctx.UserForm.Gender = Gender.Female;
        return new ValueTask<State>(State.NameInput);
    }
}