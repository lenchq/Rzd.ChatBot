using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class EditPhotoContinueDialogue : PhotoOrActionDialogue
{
    public override IEnumerable<BotAction> Options { get; set; }

    public override InputType InputType => InputType.Photo
                                           | InputType.Option;

    public EditPhotoContinueDialogue()
        : base("editPhoto")
    {
        Options = new BotAction[] {Continue};
    }

    [OptionIndex(0)]
    static ValueTask<State> Continue(Context ctx) => new ValueTask<State>(State.FormAlmostReady);

    public override State State => State.PhotoInputContinue;

    public override ValueTask<State> ProceedInput(Context ctx, Photo photo)
    {
        var photoId = photo.FileId;
        ctx.UserForm.Photos = ctx.UserForm.Photos
            .Append(photoId)
            .ToArray();
        
        ctx.UserContext.PhotoCount = ctx.UserForm.Photos.Length;

        if (ctx.UserContext.PhotoCount == 3)
        {
            return ValueTask.FromResult(State.FormAlmostReady);
        }

        return ValueTask.FromResult(State.PhotoInputContinue);
    }

    protected override bool ValidatePhoto(Context ctx, Photo photo)
    {
        var currPhotos = ctx.UserForm.Photos.Length + 1;
        
        return photo.Height is > 300 and < 2040 
               && photo.Width is > 300 and < 2040
               && currPhotos <= 3;
    }

    public override string GetText(Context ctx)
    {
        return LocalizationWrapper.FormattedText("addMorePhoto", ctx.UserContext.PhotoCount);
    }
}