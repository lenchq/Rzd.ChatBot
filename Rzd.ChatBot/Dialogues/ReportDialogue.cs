using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class ReportDialogue : ActionDialogue
{
    private IReportRepository _repo;

    public ReportDialogue() : base("report")
    {
        Options = new BotAction[] {Pornography, Reseller, WontAnswer, Other, Cancel};
    }

    protected override void Initialized()
    {
        _repo = GetService<IReportRepository>();
    }

    [OptionIndex(0)]
    async ValueTask<State> Pornography(Context ctx)
    {
        if ((long?) ctx.UserContext.CustomDataHolder is null)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("Something went wrong when reporting, {ctx}", ctx);
            return State.ThanksForReportRedir;
        }
        await SendReport(_repo, LocalizationWrapper, ctx, ReportType.Pornography);
        return State.ThanksForReportRedir;
    }

    [OptionIndex(1)]
    async ValueTask<State> Reseller(Context ctx)
    {
        if ((long?) ctx.UserContext.CustomDataHolder is null)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("Something went wrong when reporting, {ctx}", ctx);
            return State.ThanksForReportRedir;
        }
        await SendReport(_repo, LocalizationWrapper, ctx, ReportType.Reseller);
        return State.ThanksForReportRedir;
    }

    [OptionIndex(2)]
    async ValueTask<State> WontAnswer(Context ctx)
    {
        if ((long?) ctx.UserContext.CustomDataHolder is null)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("Something went wrong when reporting, {ctx}", ctx);
            return State.ThanksForReportRedir;
        }
        await SendReport(_repo, LocalizationWrapper, ctx, ReportType.WontAnswer);
        return State.ThanksForReportRedir;
    }

    [OptionIndex(3)]
    ValueTask<State> Other(Context ctx) => new(State.ReportOther);
    [OptionIndex(4)]
    ValueTask<State> Cancel(Context ctx) => new(State.Menu);


    public static async Task SendReport(IReportRepository repo, LocalizationWrapper wrapper, Context ctx, ReportType reportType, string? reason = null)
    {
        if (reportType is not ReportType.Other)
            reason ??= wrapper.Text(reportType.ToString());

        await repo.CreateReportAsync(new Report
        {
            From = ctx.UserContext.Id,
            Target = (long) ctx.UserContext.CustomDataHolder!,
            Reason = reason,
            Type = reportType,
        });
    }


    public override State State => State.Report;
}