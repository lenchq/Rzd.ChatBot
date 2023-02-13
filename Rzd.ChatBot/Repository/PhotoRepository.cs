// using Microsoft.Extensions.Options;
// using Rzd.ChatBot.Repository.Interfaces;
// using Rzd.ChatBot.Types;
// using Rzd.ChatBot.Types.Options;
//
// namespace Rzd.ChatBot.Repository;
//
// public class PhotoRepository : IPhotoRepository
// {
//     private readonly string _basePath;
//     
//     public PhotoRepository(IOptions<FileUploadOptions> options)
//     {
//         _basePath = options.Value.Path;
//         CreateDirectory();
//     }
//
//     private void CreateDirectory()
//     {
//         Directory.CreateDirectory(_basePath);
//     }
//
//     public ValueTask<Photo> GetPhotoAsync(string fileId)
//     {
//         var filePath = Path.Join(_basePath, fileId + ".jpg");
//         using var fileStream = File.Open(filePath, FileMode.Open);
//         return ValueTask.FromResult( new Photo(fileId, fileStream) );
//     }
//
//     public ValueTask SavePhotoAsync(string fileId, Stream data)
//     {
//         var filePath = Path.Join(_basePath, fileId + ".jpg");
//         using var fileStream = File.Create(filePath);
//         using var sw = new StreamWriter(fileStream);
//         sw.Write(data);
//         return ValueTask.CompletedTask;
//     }
// }