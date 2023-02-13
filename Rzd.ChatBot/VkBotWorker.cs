using Microsoft.Extensions.Options;
using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types.Options;
using VkNet;
using Rzd.ChatBot.Types;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkMessage = VkNet.Model.Message;
using VkNet.Model;
using VkNet.Model.Keyboard;
using File = System.IO.File;

namespace Rzd.ChatBot;

public sealed class VkBotWorker : BotWorker
{
    private const string RedisPrefix = "vk:";
    
    private ulong _lastTs;
    private VkApi _client;
    private readonly Random _random;
    private const string TsFilename = "vk_ts";

    public VkBotWorker(
        ILogger<VkBotWorker> logger,
        IOptions<VkBotOptions> options,
        IUserContextRepository contextRepository,
        AppLocalization localization,
        BotDialogues dialogues,
        IServiceProvider provider,
        IUserRepository userRepository
    )
    : base(RedisPrefix, logger, contextRepository, localization, dialogues, provider, userRepository)
    {
        _client = new VkApi();
        _random = new Random();
        _client.Authorize(new()
        {
            AccessToken = options.Value.Token,
        });
        _lastTs = LoadLastTs() ?? FetchTs();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            LongPollHistoryResponse poll;
            try
            {
                poll = _client.Messages.GetLongPollHistory(new()
                {
                    Ts = _lastTs,
                });
            }
            catch (VkApiMethodInvokeException)
            {
                Logger.LogWarning("Wrong Ts value, resetting Ts");
                _lastTs = FetchTs();
                poll = _client.Messages.GetLongPollHistory(new()
                {
                    Ts = _lastTs,
                });
                SaveLastTs();
            }


            if (poll.UnreadMessages == 0)
            {
                await Task.Delay(100, stoppingToken);
                continue;
            }

            for (var i = 0; i < poll.Messages.Count; i++)
            {
                var msg = poll.Messages[i];

                ArgumentNullException.ThrowIfNull(msg.FromId);
                var userCtx = GetUserContext(msg.FromId.Value);

                var context = ContextFactory.FromVk(msg, userCtx);
                
                await HandleMessageAsync(context);
            }

            _lastTs = ulong.Parse(_client.Messages.GetLongPollServer().Ts);
        }
    }

    

    protected override async Task SendTextMessage(long chatId, string text, OptionsProvider? provider)
    {
        MessageKeyboard? keyboard = null; 
        if (provider?.Options is not null)
        {
            
            keyboard = new MessageKeyboard
            {
                Buttons = provider.Options.Select((row, i) =>
                {
                    return row.Select((action, j) => new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = action
                        },
                        Color = SafetyEnum<KeyboardButtonColor>.FromJsonString(provider.Colors[i][j]) ??
                                KeyboardButtonColor.Default
                    }).ToArray();
                }),
                Inline = false,
                OneTime = true,
            };
        }
        
        
        
        await _client.Messages.SendToUserIdsAsync(new()
        {
            RandomId = _random.Next(),
            UserId = chatId,
            Message = text,
            Keyboard = keyboard,
        });
        //_logger.LogDebug("SENT MESSAGE TO {0}", chatId);
    }

    public override void Dispose()
    {
        SaveLastTs();
        base.Dispose();
    }

    private void SaveLastTs()
    {
        File.WriteAllText(TsFilename, _lastTs.ToString());
    }
    private ulong? LoadLastTs()
    {
        if (!File.Exists(TsFilename))
            return null;
        
        return ulong.Parse(File.ReadAllText(TsFilename));
    }
    private ulong FetchTs()
        => ulong.Parse( _client.Messages.GetLongPollServer().Ts );
}