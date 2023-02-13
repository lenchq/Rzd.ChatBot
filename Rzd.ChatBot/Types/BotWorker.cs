using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class BotWorker : BackgroundService
{
    protected readonly BotDialogues Dialogues;
    protected readonly IUserContextRepository ContextRepository;
    protected readonly AppLocalization Localization;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger<BotWorker> Logger;
    protected readonly IUserRepository UserRepository;


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
        
        var dialogue = Dialogues.GetDialogueByState(userCtx.State);

        // TODO: resolve contextual buttons
        // TODO: extract method ResolveOption() 
        if (userCtx.InputType == InputType.Option && msg.Text is not null)
        {
            var actionDialogue = (ActionDialogue) dialogue;
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
                var nextState = action.Invoke(ctx);
                userCtx.State = nextState;
                
                await Finalize(nextState);
            }
            // exclude commands
            // else if (!msg.Text.StartsWith('/'))
            // {
            //     //var options = GetOptions(actionDialogue);
            //     var options = actionDialogue.GetOptions();
            //     await SendTextMessage(chatId,
            //         actionDialogue.WrongAnswerText(ctx),
            //         options);
            // }
        }
        //TODO extract method ResolveTextInput()
        else if (userCtx.InputType == InputType.Text)
        {
            var inputDialogue = (InputDialogue) dialogue;
            if (!inputDialogue.Validate(ctx.Message))
            {
                await SendTextMessage(chatId,
                    inputDialogue.GetErrorText(ctx));
                return;
            }

            var nextState = inputDialogue.ProceedInput(ctx);
            userCtx.State = nextState;

            
            await Finalize(nextState);
        }
        else if (userCtx.InputType == InputType.Photo)
        {
            var photoDialogue = (PhotoDialogue) dialogue;
            if (!photoDialogue.Validate(msg, ctx))
            {
                await SendTextMessage(chatId,
                    photoDialogue.GetErrorText(ctx));
                return;
            }

            var nextState = photoDialogue.ProceedInput(ctx, msg.Photos!);
            userCtx.State = nextState;

            await Finalize(nextState);
        }
        else if (userCtx.InputType == InputType.OptionOrText)
        {
            var optionOrInputDialogue = (OptionOrInputDialogue) dialogue;
            State nextState;
            
            // Proceed options
            optionOrInputDialogue.Actions.TryGetValue(msg.Text.Trim(), out var action);
            if (int.TryParse(msg.Text, out var index)
                && index > 0
                && index - 1 < optionOrInputDialogue.Actions.Count)
            {
                action ??= optionOrInputDialogue.Actions
                    .Values
                    .ToArray()[index - 1];
            }

            if (action is not null)
            {
                nextState = action.Invoke(ctx);
                userCtx.State = nextState;
                
                await Finalize(nextState);
            }
            else
            {
                // Proceed text input
                if (!optionOrInputDialogue.Validate(ctx.Message))
                {
                    await SendTextMessage(chatId,
                        optionOrInputDialogue.GetErrorText(ctx));
                }
                nextState = optionOrInputDialogue.ProceedInput(ctx);
                userCtx.State = nextState;
                
                await Finalize(nextState);
            }
            
            
        }

        if (userCtx.Modified)
        {
            ContextRepository.SetContext(userCtx);
        }

        if (ctx.UserForm.Modified)
        {
            await UserRepository.UpdateForm(ctx.UserForm);
        }

        // ReSharper disable once LocalFunctionHidesMethod
        async Task Finalize(State nextState)
        {
            if (Dialogues.GetDialogueByState(nextState) is { } nextDialogue)
            {
                if (nextDialogue.InputType == InputType.None)
                {
                    await SendTextMessage(chatId, nextDialogue.GetText(ctx));
                    //TODO rename
                    var nextNextState = ((MessageDialogue) nextDialogue).NextState;
                    ctx.UserContext.State = nextNextState;
                    await Finalize(nextNextState).ConfigureAwait(false);
                    return;
                }
                OptionsProvider? options = null;
                if (nextDialogue.InputType is InputType.Option or InputType.OptionOrText)
                {
                    options = ((ActionDialogue) nextDialogue).GetOptions();
                }

                await SendTextMessage(chatId,
                    nextDialogue.GetText(ctx), options);
                userCtx.InputType = nextDialogue.InputType;
            }
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
                Modified = true
            };
            userCtx = newContext;
            //ContextRepository.SetContext(newContext);
        }

        return userCtx;
    }

    protected async Task<UserForm> CreateOrGetUserFormAsync(long msgFromId)
    {
        return await UserRepository.GetFormAsync(msgFromId)
                   ?? await UserRepository.CreateFormAsync(msgFromId);
    }
     
    

    protected async Task HandleCommands(Context ctx, CancellationTokenSource cts)
    {
        if (ctx.Message.Text == "/start")
        {
            //TODO не удалять контекст, а просто перемещать его на Starting, потом из 
            // Starting перемещать в меню если анкета уже заполнена. 
            ContextRepository.DeleteContext(ctx.UserContext.Id);
            
            var starting = new StartDialogue();
            starting.DependencyInjection(ServiceProvider);
            await SendTextMessage(ctx.UserContext.Id,
                starting.GetText(ctx), starting.GetOptions());
            cts.Cancel();
        }
        //TODO: another commands, /help, /about....et....c..
    }
    
    protected async Task SendTextMessage(long chatId, string text)
    {
        await this.SendTextMessage(chatId, text, null);
    }

    
    // protected async Task SendTextMessage(long chatId, string text, IEnumerable<string>? flatOptions = null)
    // {
    //     await this.SendTextMessage(chatId, text,
    //         flatOptions is null ? null : new[] {flatOptions});
    // }
    protected abstract Task SendTextMessage(long chatId, string text, OptionsProvider? prov);
    // protected virtual IEnumerable<string> GetOptions(ActionDialogue actionDialogue)
    //     => actionDialogue.Actions.Keys;
}