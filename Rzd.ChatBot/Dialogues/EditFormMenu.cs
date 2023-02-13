using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class EditFormMenuDialogue : ActionDialogue
{
    public override State State => State.EditFormMenu;
    public EditFormMenuDialogue() : base("editForm")
    {
        Options = new BotAction[] { /* RefillForm , */ EditAbout, EditPermissions, EditPhotos, EditTrainPlace, GoBack, };
    }

    // [OptionIndex(0)]
    // ValueTask<State> RefillForm(Context ctx)
    // {
    //     //TODO remove all user photos (or whole user info?)
    //     return new ValueTask<State>(State.Starting);
    // }

    [OptionIndex(0)]
    ValueTask<State> EditAbout(Context ctx) => new(State.FormEditAbout);


    [OptionIndex(1)]
    ValueTask<State> EditPhotos(Context ctx) => new(State.FormEditPhoto);

    [OptionIndex(2)]
    ValueTask<State> EditTrainPlace(Context ctx) => new(State.FormRescanQr);

    [OptionIndex(3)]
    ValueTask<State> EditPermissions(Context ctx) => new(State.FormEditPermissions);


    [OptionIndex(4)]
    ValueTask<State> GoBack(Context ctx)
    {
        if (ctx.UserForm.Disabled)
        {
            // kostyl
            ctx.UserForm.Disabled = false;
            ctx.UserForm.Fulfilled = true;
        }
        return new(State.Menu);
    }
}