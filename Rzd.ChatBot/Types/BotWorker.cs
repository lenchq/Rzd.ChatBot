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

namespace Rzd.ChatBot.Types;

public abstract class BotWorker : BackgroundService
{
    protected readonly BotDialogues Dialogues;
    protected readonly IUserContextRepository ContextRepository;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IUserRepository UserRepository;
    
    protected readonly AppLocalization Localization;
    protected readonly ILogger<BotWorker> Logger;
    


    protected BotWorker(
        string redisBotPrefix,
        ILogger<BotWorker> logger,
        IUserContextRepository contextRepository,
        AppLocalization localization,
        BotDialogues dialogues,
        IServiceProvider serviceProvider,
        IUserRepository userRepository
        )
    {
        Logger = logger;
        ContextRepository = contextRepository;
        Localization = localization;
        Dialogues = dialogues;
        ServiceProvider = serviceProvider;
        UserRepository = userRepository;
        
        ContextRepository.Initialize(redisBotPrefix);
    }

    protected async Task OnFormMatch(UserContext matchTarget, UserForm matcher)
    {
        if (matchTarget.State == State.Browsing)
        {
            // TODO отложить 
        }
        
        
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
                    photos: nextDialogue is IPhotoMessage photoMessage ? photoMessage.GetPhotos(ctx) : null);
                
                //TODO rename
                var nextNextState = ((IMessageDialogue) nextDialogue).NextState;
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
        if (ctx.Message.Text == "/start")
        {
            //TODO не удалять контекст, а просто перемещать его на Starting, потом из 
            //TODO Starting перемещать в меню если анкета уже заполнена. 

            if (ctx.UserForm.Fulfilled)
            {
                var startingRedir = new StartRedirDialogue();
                
                ctx.UserContext.State = startingRedir.State;
                ctx.UserContext.InputType = startingRedir.InputType;

                startingRedir.DependencyInjection(ServiceProvider);
                await SendTextMessage(ctx.UserContext.Id, startingRedir.GetText(ctx),
                    startingRedir.GetOptions(), startingRedir.GetPhotos());

                await ContextRepository.SetContextAsync(ctx.UserContext);
                
                cts.Cancel();
                return;
            }
            
            ContextRepository.DeleteContext(ctx.UserContext.Id);
            
            var starting = new StartDialogue();
            starting.DependencyInjection(ServiceProvider);
            await SendTextMessage(ctx.UserContext.Id,
                starting.GetText(ctx), starting.GetOptions(), starting.GetPhotos());
            cts.Cancel();
        }
        else if (ctx.Message.Text == "/dev_delete_me")
        {
            ContextRepository.DeleteContext(ctx.UserContext.Id);
            await UserRepository.DeleteForm(ctx.UserContext.Id);

            var deleted = new _DeletedMessage();
            deleted.DependencyInjection(ServiceProvider);
            await SendTextMessage(ctx.UserContext.Id,
                deleted.GetText(ctx));
            cts.Cancel();
        }
        else if (ctx.Message.Text!.Contains("/dev_encode"))
        {
            if (ctx.Message.Text.Split(' ').Length < 2)
                return;
            var payload = ctx.Message.Text.Split(' ')[1];
            
            var opts = ServiceProvider.GetService<IOptions<QrOptions>>()!.Value;
            var encrypted = Crypto.AES.Encrypt(payload, opts.SecretKey);
            await SendTextMessage(ctx.UserContext.Id, encrypted);
        }
        else if (ctx.Message.Text!.Contains("/dev_fake_gen"))
        {
            if (ctx.Message.Text.Split(' ').Length < 2)
                return;
            var payload = int.Parse( ctx.Message.Text.Split(' ')[1] );

            var users = new List<UserForm>();
            var l = new List<UserLike>();
            var trainNums = new[] {"123Б"/*, "442У", "144Ю", "333К"*/};
            
            
            var faker = new Faker<UserForm>("ru")
                // .StrictMode(true)
                .RuleFor(_ => _.Id, f => f.Random.Int())
                .RuleFor(_ => _.Gender, f => (Gender) f.Random.Int(0,1))
                .RuleFor(_ => _.Name, (f,form) => f.Name.FullName((Name.Gender?) form.Gender))
                .RuleFor(_ => _.About, f => f.Commerce.ProductDescription())
                .RuleFor(_ => _.Age, f => f.Random.Int(13, 99))
                .RuleFor(_ => _.TrainNumber, f => f.PickRandom(trainNums))
                .RuleFor(_ => _.ShowContact, true)
                .RuleFor(_ => _.ShowCoupe, true)
                .RuleFor(_ => _.Seat, f => f.Random.Int(0, 100))
                .RuleFor(_ => _.Photos, f => f.Make(f.Random.Int(1, 3), _ => f.Image.LoremFlickrUrl(1280, 720)).ToArray())
                .RuleFor(_ => _.Disabled, false);

            for (int i = 0; i < payload; i++)
            {
                var gender = Random.Shared.Next(0, 1);


                var user = faker.Generate();

                await UserRepository.CreateFormAsync(user);

                var lc = Random.Shared.Next(0, await UserRepository.FormsCount()-1);

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
        else if (ctx.Message.Text!.Contains("/dev_getid"))
        {
            if (ctx.Message.Photo is { } photo)
            {
                await SendTextMessage(ctx.UserContext.Id, ctx.Message.Photo.FileId);
            } 
        }
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

    protected abstract Task SendTextMessage(long chatId, string text, OptionsProvider? prov = null,
        Photo[]? photos = null);

    protected abstract Task<Stream> GetFile(string fileId);
}