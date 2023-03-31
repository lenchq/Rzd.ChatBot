using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class ReportThanks : MessageDialogue
{
    public ReportThanks() : base("reportThanks")
    {
    }

    public override State State => State.ThanksForReportRedir;
    public override State NextState(Context ctx) => State.Menu;
}