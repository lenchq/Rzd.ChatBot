using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class MyFormMessage : PhotoMessage
{
    public override State State => State.MyForm;

    public override State NextState { get; set; } = State.EditFormOrContinue;

    public MyFormMessage()
        : base("myForm")
    {
    }

    public override string GetText(Context ctx)
    {
        return LocalizationWrapper.FormattedText("caption", ctx.UserForm);
    }
    public override Photo[] GetPhotos(Context ctx)
    {
        return ctx.UserForm.Photos
            .Select(photo => new Photo(photo))
            .ToArray();
    }
}