using System.Text.RegularExpressions;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class EditTrainNumberDialogue : InputDialogue
{
    public static Regex TrainNumberRegex = new Regex(@"\d\d\d[А-Я]", RegexOptions.IgnoreCase);
    public override State State => State.AddTrainNumber;

    public EditTrainNumberDialogue()
        : base("editTrainNumber")
    {

    }

    public override ValueTask<State> ProceedInput(Context ctx)
    {
        var trainNumGroup = TrainNumberRegex.Match(ctx.Message.Text).Groups[0];
        var trainNum = trainNumGroup.Value.ToUpper();
        ctx.UserForm.TrainNumber = trainNum;

        return ValueTask.FromResult(State.AddCoupe);
    }


    public override bool Validate(Message msg)
    {
        return TrainNumberRegex.IsMatch(msg.Text);
    }
}