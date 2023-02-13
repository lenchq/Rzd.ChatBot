namespace Rzd.ChatBot.Types;

public class Photo
{
    public string FileId { get; set; }
    
    //TODO move Width and Height to PhotoData
    public int Width { get; set; }
    public int Height { get; set; }

    public Photo()
    {
    }

    public Photo(string fileId)
    {
        this.FileId = fileId;
    }
}

public class PhotoData : Photo
{
    public Stream FileData { get; set; } = null!;
    public int Size { get; set; }
    public string FileName { get; set; } = null!;
}