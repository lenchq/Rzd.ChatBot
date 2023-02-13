using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Types.Interfaces;

public interface IPhotoMessage: IMessageDialogue
{
    Photo[] GetPhotos(Context ctx);
}