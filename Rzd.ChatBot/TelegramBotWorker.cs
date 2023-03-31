using Microsoft.Extensions.Options;
using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
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
    private readonly ReceiverOptions _receiverOptions;
    private readonly bool _isDev;
    private readonly TelegramContextFactory _contextFactory;

    public TelegramBotWorker(
        BotDialogues dialogues,
        ILogger<TelegramBotWorker> logger,
        IOptions<TelegramBotOptions> options,
        IUserContextRepository contextRepository,
        AppLocalization localization,
        IServiceProvider provider,
        IUserRepository userRepository,
        IOptions<QrOptions> qrOptions,
        IWebHostEnvironment env
    )
    : base(RedisPrefix, logger, contextRepository, localization, dialogues, provider, userRepository, qrOptions)
    {
        var opts = options.Value;
        _receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = opts.Updates
                .Select(Enum.Parse<UpdateType>)
                .ToArray(),
        };
        _client = new TelegramBotClient(opts.Token);
        _contextFactory = new TelegramContextFactory(_client);
        _isDev = env.IsDevelopment();
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
        //await _client.DeleteMyCommandsAsync();
        // await _client.SetMyCommandsAsync(GetCommandList()
        //     .Select(x => new BotCommand() {Command = x.Key, Description = x.Value}), cancellationToken: cancellationToken);
        if (update.CallbackQuery is not null)
        {
            var userId = update.CallbackQuery.From.Id;
            var _userCtx = GetUserContext(userId);
            var ctx = new Context
            {
                UserContext = _userCtx,
            };

            await HandleInlineQueryAsync(ctx, update.CallbackQuery.Data);
            await _client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: cancellationToken);
            return;
        }
        else if (update.Message?.Text is null && update.Message?.Photo is null)
            return;
        
        var msg = update.Message!;
        var chatId = msg.Chat.Id;
        
        var userCtx = GetUserContext(chatId);
        userCtx.Modified = false;
        var userForm = await CreateOrGetUserFormAsync(chatId);

        if (userForm.Username is null && msg.From?.Username is not null)
        {
            userForm.Username = msg.From.Username;
            userForm.Modified = true;
        }

        var context = _contextFactory.CreateContext(msg, userCtx, userForm);

        try
        {
            // throw new NotImplementedException();
            await HandleMessageAsync(context);
        }
        catch (Exception e)
        {
            Logger.LogError(e,"Something went wrong when processing user id({@Id}) message",
                context.UserContext.Id);
        }
    }
    
    protected override async Task SendTextMessage(long chatId, string text,
        OptionsProvider? provider = null, Photo[]? photos = null, InlineButtonsProvider? inlines = null)
    {
        // ParseMode? parseMode = useHtml is not null && useHtml.Value ? ParseMode.Html : null;
        ParseMode? parseMode = ParseMode.Html;
        
        
        IReplyMarkup? markup = null;
        if (provider?.Options is not null && provider.Options.Any())
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

        if (provider is not OptionsPreserve)
            markup ??= new ReplyKeyboardRemove();

        if (inlines is not null)
            markup = new InlineKeyboardMarkup(
                inlines.Buttons.Select(row =>
                {
                    return row.Select(x => 
                        InlineKeyboardButton.WithCallbackData(x.Text, x.Key));
                })
            );
        
        if (photos is null)
        {
            await _client.SendTextMessageAsync(chatId, text, replyMarkup: markup, parseMode: parseMode);
            return;
        }

        if (photos.Length == 1)
        {
            var photo = photos.First();

            await _client.SendPhotoAsync(chatId, photo is PhotoData innerPhoto
                    ? new InputMedia(innerPhoto.FileData, innerPhoto.FileName)
                    : photo.FileId,
                caption: text, replyMarkup: markup, protectContent: false);
        }
        else
        {
            await _client.SendMediaGroupAsync(chatId,
                photos
                    .Select(photo => new InputMediaPhoto(
                        photo is PhotoData innerPhoto
                            ? new InputMedia(innerPhoto.FileData, innerPhoto.FileName)
                            : photo.FileId
                    )),
                disableNotification: true, protectContent: false);

            await _client.SendTextMessageAsync(chatId, text, replyMarkup: markup, parseMode: parseMode);
        }
    }

    protected override async Task<Stream> GetFile(string fileId)
    {
        Stream fileStream = new MemoryStream();
        await _client.GetInfoAndDownloadFileAsync(fileId, fileStream);
        fileStream.Seek(0, SeekOrigin.Begin);
        return fileStream;
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        if (_isDev)
            throw exception;
        else
            Logger.LogError(exception, "Exception occured:");

        return Task.CompletedTask;
    }
}