using Microsoft.Extensions.Options;
using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramMessage = Telegram.Bot.Types.Message;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Rzd.ChatBot;

public sealed class TelegramBotWorker : BotWorker
{
    private const string RedisPrefix = "tg:";
    
    private readonly TelegramBotClient _client;
    private readonly TelegramBotOptions _options;
    private readonly ReceiverOptions _receiverOptions;
    private TelegramContextFactory _contextFactory;

    public TelegramBotWorker(
        BotDialogues dialogues,
        ILogger<TelegramBotWorker> logger,
        IOptions<TelegramBotOptions> options,
        IUserContextRepository contextRepository,
        AppLocalization localization,
        IServiceProvider provider,
        IUserRepository userRepository
    )
    : base(RedisPrefix, logger, contextRepository, localization, dialogues, provider, userRepository)
    {
        _options = options.Value;
        _receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = _options.Updates
                .Select(Enum.Parse<UpdateType>)
                .ToArray(),
        };
        _client = new TelegramBotClient(_options.Token);
        _contextFactory = new TelegramContextFactory(_client);
    }

   

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _client.ReceiveAsync(
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                _receiverOptions,
                stoppingToken
                );
        }
    }


    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
        )
    {
        if (update.Message?.Text is null && update.Message?.Photo is null)
            return;
        
        var msg = update.Message!;
        var chatId = msg.Chat.Id;
        
        var userCtx = GetUserContext(chatId);
        var userForm = await CreateOrGetUserFormAsync(chatId);
        
        var context = _contextFactory.CreateContext(msg, userCtx, userForm);

        await HandleMessageAsync(context);
    }
    
    protected override async Task SendTextMessage(long chatId, string text, OptionsProvider? provider)
    {
        IReplyMarkup? markup = null;
        if (provider?.Options is not null)
        {
            markup = new ReplyKeyboardMarkup(
                provider.Options.Select(row =>
                {
                    return row.Select(x => new KeyboardButton(x));
                })
            )
            {
                ResizeKeyboard = true,
                InputFieldPlaceholder = Localization.GetItem("inputPlaceholder"),
            };
        }
        
        markup ??= new ReplyKeyboardRemove();
        await _client.SendTextMessageAsync(chatId, text, replyMarkup: markup);
    }
    private async Task SendFormatMessage(long chatId, string text, params object[]? args)
    {
        var formatted = string.Format(text, args!);
        await _client.SendTextMessageAsync(chatId, formatted);
    }
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        throw exception;
    }
}