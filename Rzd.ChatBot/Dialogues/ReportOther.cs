using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class ReportOther : OptionOrInputDialogue
{
    private IReportRepository _repo;

    public ReportOther() : base("reportOther")
    {
        Options = new BotAction[] { GoBack };
    }

    protected override void Initialized()
    {
        _repo = GetService<IReportRepository>();
    }

    [OptionIndex(0)]
    ValueTask<State> GoBack(Context ctx) => new(State.Menu);

    public override State State => State.ReportOther;
    public override async ValueTask<State> ProceedInput(Context ctx)
    {
        await ReportDialogue.SendReport(_repo, LocalizationWrapper, ctx, ReportType.Other, ctx.Message.Text);
        return State.ThanksForReportRedir;
    }

    public override bool Validate(Message msg)
    {
        return !string.IsNullOrWhiteSpace(msg.Text);
    }
}