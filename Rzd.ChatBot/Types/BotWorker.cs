using Bogus;
using Bogus.DataSets;
using Microsoft.Extensions.Options;
using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;
using Rzd.ChatBot.Types.Options;
using SmartFormat;
using SmartFormat.Core.Parsing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class BotWorker : BackgroundService
{
    protected readonly BotDialogues Dialogues;
    protected readonly IUserContextRepository ContextRepository;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IUserRepository UserRepository;
    private readonly string _qrSecret;
    
    protected readonly AppLocalization Localization;
    protected readonly ILogger<BotWorker> Logger;

    protected BotWorker(
        string redisBotPrefix,
        ILogger<BotWorker> logger,
        IUserContextRepository contextRepository,
        AppLocalization localization,
        BotDialogues dialogues,
        IServiceProvider serviceProvider,
        IUserRepository userRepository,
        IOptions<QrOptions> qrOptions)
    {
        Logger = logger;
        ContextRepository = contextRepository;
        Localization = localization;
        Dialogues = dialogues;
        ServiceProvider = serviceProvider;
        UserRepository = userRepository;

        _qrSecret = qrOptions.Value.SecretKey;
        ContextRepository.Initialize(redisBotPrefix);
        MatchHook.Instance.Like += OnFormLike;
        MatchHook.Instance.Match += OnFormMatch;
    }

    private async ValueTask OnFormMatch(MatchEventArgs args)
    {
        var brow = new Browsing();
        brow.DependencyInjection(ServiceProvider);
        
        var (from, to) = args;
        var form = await UserRepository.GetFormAsync(from);
        var text = Smart.Format(Localization["match:caption"],
            new
            {
                form.Seat,
                form.TrainNumber,
                form.Name,
                form.ShowCoupe,
                form.ShowContact,
                form.Username,
                Contact = form.Id,
            });

        string Unescape()
        {
            
            System.Span<char> buff1 = stackalloc char[text.Length];
            EscapedLiteral.UnEscapeCharLiterals('\\', text, false, buff1);
            return buff1.ToString();
        }

        text = Unescape();
        await SendTextMessage(to, text, OptionsProvider.Preserve, inlines: new InlineButtonsProvider()
        {
            Buttons = new[]
            {
                new []
                {
                    ($"report:{form.Id}", Localization["match:report"])
                }
            }
        });
    }

    private async ValueTask OnFormLike(LikeEventArgs args)
    {
        var (fromId, toId) = args;
        var toCtx = await ContextRepository.GetContextAsync(toId);
        toCtx.LikeQueue.Enqueue(fromId);
        var message = Localization["browsing:like"];

        try
        {
            if (toCtx is {State: State.Browsing, LikeQueue.Count: > 1})
            {
                message = string.Format(Localization["browsing:like_browsing_several"], toCtx.LikeQueue.Count);
            }
            else if (toCtx.State == State.Browsing)
            {
                message = Localization["browsing:like_browsing"];
            }
            else if (toCtx.LikeQueue.Count == 1)
            {
                message = Localization["browsing:like"];
            }
            else if (toCtx.LikeQueue.Count > 1 && (toCtx.LikeQueue.Count & 1) == 1)
            {
                message = string.Format(Localization["browsing:like_several"], toCtx.LikeQueue.Count);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when formatting like string id({0})", toCtx.Id);
            message = Localization["browsing:like"];
        }

        await SendTextMessage(toCtx.Id, message, OptionsProvider.Preserve);
        
        await UpdateUserContextIfModified(toCtx);
    }

    protected async Task HandleMessageAsync(Context ctx)
    {
        using (var commandsCts = new CancellationTokenSource())
        {
            await HandleCommands(ctx, commandsCts);
            if (commandsCts.IsCancellationRequested)
                return;
        }
        var userCtx = ctx.UserContext;
        var msg = ctx.Message;
        var chatId = userCtx.Id;
        
        var dialogue = Dialogues.NewDialogueByState(userCtx.State);
        await dialogue.PostInitializeAsync(ctx);

        // TODO: resolve contextual buttons
        var finalized = false;

        if (userCtx.InputType.HasFlag(InputType.Option) && msg.Text is not null)
        {
            var actionDialogue = (IActionDialogue) dialogue;
            actionDialogue.Actions.TryGetValue(msg.Text.Trim(), out var action);
            if (int.TryParse(msg.Text, out var index)
                && index > 0
                && index - 1 < actionDialogue.Actions.Count)
            {
                action ??= actionDialogue.Actions
                    .Values
                    .ToArray()[index - 1];
            }

            if (action is not null)
            {
                var nextState = await action.Invoke(ctx);

                await Finalize(nextState);
            }
        }
        
        
        if (userCtx.InputType.HasFlag(InputType.Text) && !finalized)
        {
            var inputDialogue = (IInputDialogue) dialogue;
            if (!inputDialogue.Validate(ctx.Message))
            {
                await SendTextMessage(chatId,
                    inputDialogue.GetErrorText(ctx));
                return;
            }

            var nextState = await inputDialogue.ProceedInput(ctx);

            await Finalize(nextState);
        }
        else if (userCtx.InputType.HasFlag(InputType.Photo) && !finalized
                 && msg.Photo is not null)
        {
            var photoDialogue = (IPhotoDialogue) dialogue;

            var photo = photoDialogue.SupportsPhotoData
                ? await GetPhotoData(msg.Photo)
                : msg.Photo;
            
            if (!photoDialogue.Validate(ctx, photo))
            {
                await SendTextMessage(chatId, 
                    photoDialogue.GetErrorText(ctx));
                return;
            }
            
            var nextState = await photoDialogue.ProceedInput(ctx, photo);

            await Finalize(nextState);
        }

        await UpdateUserContextIfModified(ctx.UserContext);
        await UpdateUserFormIfModified(ctx.UserForm);

        // ReSharper disable once LocalFunctionHidesMethod
        async Task Finalize(State nextState)
        {
            finalized = true;
            // userCtx.State = userCtx.OverrideNextState ?? nextState;
            userCtx.State = nextState;
            var nextDialogue = Dialogues.NewDialogueByState(nextState);
            await nextDialogue.PostInitializeAsync(ctx);
            // if state was overriden
            // if (userCtx.OverrideNextState is not null)
            // {
            //     userCtx.InputType = Dialogues.DialogueByState(userCtx.OverrideNextState.Value).InputType;
            // }
            // userCtx.OverrideNextState = null;
            
            // For message dialogues
            if (nextDialogue.InputType == InputType.None)
            {
                await SendTextMessage(chatId, nextDialogue.GetText(ctx),
                    photos: nextDialogue is IPhotoMessage photoMessage ? photoMessage.GetPhotos(ctx) : null,
                    inlines: nextDialogue.GetInlineButtons());
                
                //TODO rename
                var nextNextState = ((IMessageDialogue) nextDialogue).NextState(ctx);
                userCtx.State = nextNextState;
                await Finalize(nextNextState).ConfigureAwait(false);
                return;
            }
            
            OptionsProvider? options = null;
            if (nextDialogue.InputType.HasFlag(InputType.Option))
            {
                options = ((IActionDialogue) nextDialogue).GetOptions();
            }
            
            await SendTextMessage(chatId,
                 nextDialogue.GetText(ctx), options, nextDialogue.GetPhotos());
            userCtx.InputType = nextDialogue.InputType;
        }
    }

    private async Task UpdateUserFormIfModified(UserForm form)
    {
        if (form.Modified)
        {
            await UserRepository.UpdateForm(form);
        }
    }

    public async Task UpdateUserContextIfModified(UserContext ctx)
    {
        if (ctx.Modified)
        {
            await ContextRepository.SetContextAsync(ctx);
        }
    }

    protected UserContext GetUserContext(long msgFromId)
    {
        var success = ContextRepository.TryGetContext(msgFromId, out var userCtx);
        if (!success)
        {
            var newContext = new UserContext
            {
                Id = msgFromId,
                
                // true to make sure new context going to be set
                Modified = true
            };
            userCtx = newContext;
        }

        return userCtx;
    }

    protected async Task<UserForm> CreateOrGetUserFormAsync(long msgFromId)
    {
        return await UserRepository.GetFormAsync(msgFromId)
                   ?? await UserRepository.CreateFormAsync(msgFromId);
    }

    private async Task HandleCommands(Context ctx, CancellationTokenSource cts)
    {
        if (ctx.Message.Text is null) return;
        if (ctx.Message.Text!.StartsWith("/start"))
        {
            string? parameterEncrypted = null;
            if (ctx.Message.Text!.Split(' ').Length >= 2)
                parameterEncrypted = ctx.Message.Text.Split(' ')[1];
            string? parameterDecrypted = null;

            try
            {
                parameterDecrypted = Crypto.AES.Decrypt(parameterEncrypted, _qrSecret);
            }
            catch { }
            if (ctx.UserForm.Fulfilled)
            {
                if (ctx.UserForm.Disabled)
                {
                    ctx.UserForm.Disabled = false;
                    // var disabledDiag = new FormDisabled();
                    // disabledDiag.DependencyInjection(ServiceProvider);
                    //
                    // await SendTextMessage(ctx.UserContext.Id, disabledDiag.GetText(ctx), disabledDiag.GetOptions());
                    // return;
                }

                var startingRedir = new StartRedirDialogue();
                
                ctx.UserContext.State = startingRedir.State;
                ctx.UserContext.InputType = startingRedir.InputType;

                startingRedir.DependencyInjection(ServiceProvider);
                await SendTextMessage(ctx.UserContext.Id, startingRedir.GetText(ctx),
                    startingRedir.GetOptions(), startingRedir.GetPhotos());

                await ContextRepository.SetContextAsync(ctx.UserContext);
            }
            
            var starting = new StartDialogue();
            var newUserCtx = ctx.UserContext with
            {
                InputType = starting.InputType,
                State = starting.State,
                CurrentForm = null,
                LikeQueue = new Queue<long>(),
                PhotoCount = 0,
                StartData = parameterDecrypted,
            };

            await ContextRepository.SetContextAsync(newUserCtx);

            starting.DependencyInjection(ServiceProvider);
            await SendTextMessage(ctx.UserContext.Id,
                starting.GetText(ctx), starting.GetOptions(), starting.GetPhotos());
            cts.Cancel();
            return;
        }
        else if (ctx.Message.Text.StartsWith("/dev_ensure_admin"))
        {
            if (ctx.Message.Text.Split(' ').Length < 2)
                return;
            var payload = ctx.Message.Text.Split(' ')[1];
            
            var pass = ServiceProvider.GetService<IOptions<AdminOptions>>()!.Value;
            if (string.Equals(payload, pass.Password))
            {
                ctx.UserContext.IsAdmin = true;

                await UpdateUserContextIfModified(ctx.UserContext);
                await SendTextMessage(ctx.UserContext.Id, Localization["_dev_ensure_admin:caption"],
                    OptionsProvider.Preserve);
                cts.Cancel();
            }
        }
        else if (ctx.Message.Text.StartsWith("/dev_rm_admin"))
        {
            ctx.UserContext.IsAdmin = false;

            await UpdateUserContextIfModified(ctx.UserContext);
            await SendTextMessage(ctx.UserContext.Id, Localization["_dev_rm_admin:caption"],
                OptionsProvider.Preserve);
            cts.Cancel();
        }
        
        if (!ctx.UserContext.IsAdmin) return;

        #region dev commands

        if (ctx.Message.Text!.StartsWith("/dev_delete_me"))
        {
            await ContextRepository.DeleteContextAsync(ctx.UserContext.Id);
            await UserRepository.DeleteForm(ctx.UserContext.Id);

            var deleted = new _DeletedMessage();
            deleted.DependencyInjection(ServiceProvider);
            await SendTextMessage(ctx.UserContext.Id,
                deleted.GetText(ctx));
        }
        else if (ctx.Message.Text!.StartsWith("/dev_encode"))
        {
            if (ctx.Message.Text.Split(' ').Length < 2)
                return;
            var payload = ctx.Message.Text.Split(' ')[1];

            var opts = ServiceProvider.GetService<IOptions<QrOptions>>()!.Value;
            var encrypted = Crypto.AES.Encrypt(payload, opts.SecretKey);
            await SendTextMessage(ctx.UserContext.Id, encrypted, OptionsProvider.Preserve);
        }
        else if (ctx.Message.Text!.StartsWith("/dev_decode"))
        {
            if (ctx.Message.Text.Split(' ').Length < 2)
                return;
            var payload = ctx.Message.Text.Split(' ')[1];

            var opts = ServiceProvider.GetService<IOptions<QrOptions>>()!.Value;
            try
            {
                var decrypted = Crypto.AES.Decrypt(payload, opts.SecretKey);
                await SendTextMessage(ctx.UserContext.Id, decrypted);
            }
            catch (Exception ex)
            {
                await SendTextMessage(ctx.UserContext.Id, "error " + ex.Message);
            }
        }
        else if (ctx.Message.Text!.StartsWith("/dev_fake_gen"))
        {
            if (ctx.Message.Text.Split(' ').Length < 2)
                return;
            var payload = int.Parse(ctx.Message.Text.Split(' ')[1]);

            var users = new List<UserForm>();
            var l = new List<UserLike>();
            var trainNums = new[] {"123Б" /*, "442У", "144Ю", "333К"*/};


            var faker = new Faker<UserForm>("ru")
                // .StrictMode(true)
                .RuleFor(_ => _.Id, f => f.Random.Long(max: -1))
                .RuleFor(_ => _.Gender, f => (Gender) f.Random.Int(0, 1))
                .RuleFor(_ => _.Name, (f, form) => f.Name.FullName((Name.Gender?) form.Gender))
                .RuleFor(_ => _.About, f => f.Commerce.ProductDescription())
                .RuleFor(_ => _.Age, f => f.Random.Int(13, 99))
                .RuleFor(_ => _.TrainNumber, f => f.PickRandom(trainNums))
                .RuleFor(_ => _.ShowContact, true)
                .RuleFor(_ => _.ShowCoupe, true)
                .RuleFor(_ => _.Seat, f => f.Random.Int(0, 100))
                .RuleFor(_ => _.Photos,
                    f => f.Make(f.Random.Int(1, 3), _ => f.Image.LoremFlickrUrl(1280, 720)).ToArray())
                .RuleFor(_ => _.Disabled, false);

            for (var i = 0; i < payload; i++)
            {
                var user = faker.Generate();

                await UserRepository.CreateFormAsync(user);

                var lc = Random.Shared.Next(0, await UserRepository.FormsCount() - 1);

                for (int j = 0; j < lc; j++)
                {
                    var target = await UserRepository.RandomForm();
                    var like = new UserLike
                    {
                        FromId = user.Id,
                        ToId = target.Id,
                        Like = new Randomizer().Bool(.55f),
                    };

                    await UserRepository.AddLikeAsync(like);

                    l.Add(like);
                }

                users.Add(user);
            }

            var t = "";
            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];
                t += $"{user.Name} ({user.Age}), {user.Gender.ToString()}, {user.TrainNumber}\n";
            }

            if (t.Length < 5000)
                await SendTextMessage(ctx.UserContext.Id, $"Generated {payload} fake forms\n\n{t}");
        }
        else if (ctx.Message.Text!.StartsWith("/dev_getid"))
        {
            if (ctx.Message.Photo is { } photo)
            {
                await SendTextMessage(ctx.UserContext.Id, ctx.Message.Photo.FileId);
            }
        }
        cts.Cancel();
        #endregion
    }

    public Dictionary<string, string> GetCommandList()
    {
        return new Dictionary<string, string>
        {
            {"/start", "{start}"},
            {"/dev_delete_me", "delete_me"},
            {"/dev_fake_gen", "fake gen"},
            {"/dev_getid", "get id"},
            {"/dev_encode", "encode"},
        };
    }
    
    private async Task<PhotoData> GetPhotoData(Photo photo)
    {
        var file = await GetFile(photo.FileId);
        return new PhotoData
        {
            FileData = file,
            Size = (int) file.Length,
            Width = photo.Width,
            Height = photo.Height,
        };
    }

    protected async ValueTask HandleInlineQueryAsync(Context ctx, string queryData)
    {
        if (queryData.StartsWith("report"))
        {
            if (ctx.UserContext.State is State.Report or State.ReportOther)
                return;
            var arg = queryData[(queryData.IndexOf(":") + 1)..];
            var userId = long.Parse(arg);
            var reportDialogue = new ReportDialogue();
            reportDialogue.DependencyInjection(ServiceProvider);

            ctx.UserContext.State = reportDialogue.State;
            ctx.UserContext.InputType = reportDialogue.InputType;
            ctx.UserContext.CustomDataHolder = userId;

            await SendTextMessage(ctx.UserContext.Id, reportDialogue.GetText(ctx), reportDialogue.GetOptions());
            await UpdateUserContextIfModified(ctx.UserContext);
        }
    }

    protected abstract Task SendTextMessage(long chatId, string text, OptionsProvider? prov = null,
        Photo[]? photos = null, InlineButtonsProvider? inlines = null);

    protected abstract Task<Stream> GetFile(string fileId);
}