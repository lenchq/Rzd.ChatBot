using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;
using SmartFormat.Core.Parsing;

namespace Rzd.ChatBot.Dialogues;

public class MatchMessage : MessageDialogue
{
    public MatchMessage() : base("match")
    {
    }

    private UserForm form;
    private IUserRepository _repo;

    public override async ValueTask PostInitializeAsync(Context ctx)
    {
        _repo = GetService<IUserRepository>();
        form = await _repo.GetFormAsync(ctx.UserContext.LikeQueue.Peek());
    }

    public override State State => State.MatchRedir;
    public override State NextState(Context ctx)
    {
        ctx.UserContext.LikeQueue.Dequeue();
        ctx.UserContext.CurrentForm = null;

        if (!_repo.CanPickForm(ctx.UserContext.Id).GetAwaiter().GetResult())
        {
            return State.EndOfFormsRedir;
        }
        return State.Browsing;
    }

    public override string GetText(Context ctx)
    {
        var text = LocalizationWrapper.FormattedText("caption",
            new
            {
                form.TrainNumber,
                form.Seat,
                form.Name,
                form.ShowContact,
                form.ShowCoupe,
                form.Username,
                Contact = form.Id,
            });
        Span<char> buffer = stackalloc char[text.Length];
        EscapedLiteral.UnEscapeCharLiterals('\\', text, false, buffer);
        return buffer.ToString();
    }

    public override InlineButtonsProvider? GetInlineButtons()
    {
        var prov = new InlineButtonsProvider()
        {
            // Buttons = LocalizationWrapper.GetInlines()!,
            Buttons = new []
            {
                new []
                {
                    ($"report:{form.Id}", LocalizationWrapper.FormattedText("report"))
                }
            }
        };
        return prov;
    }
}