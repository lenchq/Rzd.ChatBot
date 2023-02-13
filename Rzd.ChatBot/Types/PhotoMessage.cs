using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class PhotoMessage : MessageDialogue, IPhotoMessage
{
    protected PhotoMessage(string localizationName) : base(localizationName)
    {
    }

    public abstract Photo[] GetPhotos(Context ctx);
}