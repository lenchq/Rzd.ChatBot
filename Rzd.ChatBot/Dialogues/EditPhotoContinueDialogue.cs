using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class EditPhotoContinueDialogue : PhotoDialogue
{
    public EditPhotoContinueDialogue() : base("editPhotoContinue")
    {
    }

    public override State State => State.EditPhotoContinue;
    public override State ProceedInput(Context ctx, Photo[] photos)
    {
        throw new NotImplementedException();
    }

    protected override bool ValidatePhotos(Photo[] photos, Context ctx)
    {
        throw new NotImplementedException();
    }
}