using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class EditPhotoDialogue : PhotoDialogue
{
    private int? _wrongPhoto = null;
    
    public EditPhotoDialogue()
        : base("editPhoto")
    {
    }

    public override State State => State.EditPhoto;

    public override State ProceedInput(Context ctx, Photo[] photos)
    {
        var photoIds = photos.Select(_ => _.Id);
        var p = ctx.UserForm.Photos
            .ToList();
        p.AddRange(photoIds);
        ctx.UserForm.Photos = p.ToArray();
        ctx.UserContext.PhotoCount = p.Count;

        if (p.Count == 3)
        {
            return State.FormAlmostReady;
        }

        return State.EditPhotoContinue;
    }

    protected override bool ValidatePhotos(Photo[] photos, Context ctx)
    {
        var currPhotos = ctx.UserForm.Photos.Length + photos.Length;
        return photos
                   .All(photo => photo.Height is > 300 and < 2040 && photo.Width is > 300 and < 2040)
               && currPhotos <= 3;
    }
}