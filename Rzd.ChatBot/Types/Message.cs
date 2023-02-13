using VkMessage = VkNet.Model.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace Rzd.ChatBot.Types;

public class Message
{
    public long ChatId { get; private set; }
    public string? Text { get; private set; }
    public UserInfo From { get; private set; }
    public Photo[]? Photos { get; set; }


    public static Message FromTelegramMessage(TelegramMessage message)
    {
        return new Message
        {
            ChatId = message.Chat.Id,
            Text = message.Text,
            From = new UserInfo
            {
                FirstName = message.From?.FirstName,
                LastName = message.From?.LastName,
                Username = message.From?.Username,
            },
            Photos = message.Photo?
                // Take every 4th element of collection
                // bc telegram sends 4 variations of one photo like
                // [very low res, low res, medium res, original] * photo count
                .DistinctBy(photo => photo.FileUniqueId)
                .Select(photo => new Photo
                {
                    Id = photo.FileId,
                    Width = photo.Width,
                    Height = photo.Height
                })
                .ToArray()
        };
    }

    public static Message FromVk(VkMessage message)
    {
        // TODO photos
        return new Message
        {
            ChatId = message.FromId ??
                     throw new ArgumentNullException(nameof(message.FromId)),
            Text = message.Text,
            From = new UserInfo
            {
                //TODO parse username, firstname etc from vk api (and cache it)
            }
        };
    }
}

public class UserInfo
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
}