using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class PhotoDialogue : Dialogue
{
    public override InputType InputType => InputType.Photo;

    public PhotoDialogue(string localizationName) : base(localizationName)
    {
    }

    public abstract State ProceedInput(Context ctx, Photo[] photos);

    public bool Validate(Message msg, Context ctx)
    {
        return msg.Photos is not null && ValidatePhotos(msg.Photos.ToArray(), ctx);
    }

    protected abstract bool ValidatePhotos(Photo[] photos, Context ctx);

    public virtual string GetErrorText(Context ctx)
        => LocalizationWrapper.GetText("validationError");
}