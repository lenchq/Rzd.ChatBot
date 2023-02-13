using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class PhotoDialogue : Dialogue, IPhotoDialogue
{
    public override InputType InputType => InputType.Photo;
    public virtual bool SupportsPhotoData { get; set; } = false;

    public PhotoDialogue(string localizationName) : base(localizationName)
    {
    }

    public abstract ValueTask<State> ProceedInput(Context ctx, Photo photo);

    public bool Validate(Context ctx, Photo photo)
    {
        return ctx.Message.Photo is not null && ValidatePhoto(ctx, photo);
    }

    protected abstract bool ValidatePhoto(Context ctx, Photo photo);

    public virtual string GetErrorText(Context ctx)
        => LocalizationWrapper.Text("validationError");
}