using Rzd.ChatBot.Model;
using Telegram.Bot;
using VkMessage = VkNet.Model.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace Rzd.ChatBot.Types;

public class TelegramContextFactory
{
    private readonly ITelegramBotClient _client;
    
    public TelegramContextFactory(ITelegramBotClient client)
    {
        _client = client;
    }
    
    public Context CreateContext(TelegramMessage msg, UserContext ctx, UserForm userForm)
    {
        var files = new List<string>();
        if (msg.Photo is { } photos)
        {
            // iterate every third element of collection
            // because telegram photos is [low quality, medium quality, high quality] * photo count
            for (var i = 2; i < photos.Length; i += 3)
            {
                files.Add(photos[i].FileId);
            }
        }
        

        return new Context
        {
            Message = Message.FromTelegramMessage(msg),
            UserContext = ctx,
            UserForm = userForm,
        };
    }
}