using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormEditPhotoContinue : PhotoOrActionDialogue
{
    public override IEnumerable<BotAction> Options { get; set; }
    
    public FormEditPhotoContinue() : base("editPhoto")
    {
        Options = new BotAction[] {Continue};
    }
    [OptionIndex(0)]
    private ValueTask<State> Continue(Context ctx) => new(State.MyFormEdited);

    public override State State => State.FormEditPhotoContinue;
    public override ValueTask<State> ProceedInput(Context ctx, Photo photo)
    {
        var photoId = photo.FileId;
        ctx.UserForm.Photos = ctx.UserForm.Photos
            .Append(photoId)
            .ToArray();
        
        ctx.UserContext.PhotoCount = ctx.UserForm.Photos.Length;

        if (ctx.UserContext.PhotoCount == 3)
        {
            return ValueTask.FromResult(State.MyFormEdited);
        }

        return ValueTask.FromResult(State.FormEditPhotoContinue);
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