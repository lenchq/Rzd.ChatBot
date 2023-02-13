using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types.Interfaces;

public interface IPhotoDialogue
{
    public bool SupportsPhotoData { get; set; }
    public ValueTask<State> ProceedInput(Context ctx, Photo photo);
    public bool Validate(Context ctx, Photo photo);
    public string GetErrorText(Context ctx);
}