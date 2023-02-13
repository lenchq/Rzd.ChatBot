using Rzd.ChatBot.Types;
using VkMessage = VkNet.Model.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace Rzd.ChatBot.Model;

public class Message
{
    public long ChatId { get; private set; }
    public string? Text { get; private set; }
    public UserInfo From { get; private set; } = null!;
    public Photo? Photo { get; set; }


    public static Message FromTelegramMessage(TelegramMessage message)
    {
        Photo? photo = null;
        if (message.Photo?.MaxBy(_ => _.Width * _.Height)
            is { } msgPhoto)
        {
            photo = new Photo
            {
                FileId = msgPhoto.FileId,
                Width = msgPhoto.Width,
                Height = msgPhoto.Height,
            };
        }

        return new Message
        {
            ChatId = message.Chat.Id,
            Text = message.Text ?? message.Caption,
            From = new UserInfo
            {
                FirstName = message.From?.FirstName,
                LastName = message.From?.LastName,
                Username = message.From?.Username,
            },
            Photo = photo,
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