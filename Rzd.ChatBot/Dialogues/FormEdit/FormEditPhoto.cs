using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormEditPhoto : PhotoOrActionDialogue
{
    public FormEditPhoto() : base("editPhoto")
    {
        Options = new BotAction[] {GoBack};
    }
    
    [OptionIndex(0)]
    private ValueTask<State> GoBack(Context ctx) => new ValueTask<State>(State.MyFormEdited);

    public override State State => State.FormEditPhoto;
    public override ValueTask<State> ProceedInput(Context ctx, Photo photo)
    {
        ctx.UserForm.Photos = Array.Empty<string>();
        
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
        return photo.Height is > 300 and < 2040
               && photo.Width is > 300 and < 2040;
    }

    public override IEnumerable<BotAction> Options { get; set; }
}