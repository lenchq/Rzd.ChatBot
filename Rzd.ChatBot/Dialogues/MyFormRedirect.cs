using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class MyFormRedirect : PhotoMessage
{
    public override State State => State.MyFormEdited;

    public override State NextState { get; set; } = State.EditFormMenu;

    public MyFormRedirect()
        : base("myForm")
    {
    }

    private string LocalizeGender(Gender gender)
    {
        return LocalizationWrapper.Raw($"gender:{gender.ToString().ToLower()}")!;
    }

    public override string GetText(Context ctx)
    {
        return LocalizationWrapper.FormattedText("caption",
            ctx.UserForm
        );
    }
    public override Photo[] GetPhotos(Context ctx)
    {
        return ctx.UserForm.Photos
            .Select(photo => new Photo(photo))
            .ToArray();
    }
}