using System.Text.RegularExpressions;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class EditTrainNumberDialogue : InputDialogue
{
    private readonly Regex _trainNumberRegex = new Regex(@"\d\d\d[А-Я]", RegexOptions.IgnoreCase);
    public override State State => State.AddTrainNumber;

    public EditTrainNumberDialogue()
        : base("editTrainNumber")
    {

    }

    public override ValueTask<State> ProceedInput(Context ctx)
    {
        var trainNumGroup = _trainNumberRegex.Match(ctx.Message.Text).Groups[0];
        var trainNum = trainNumGroup.Value.ToUpper();
        ctx.UserForm.TrainNumber = trainNum;

        return ValueTask.FromResult(State.AddCoupe);
    }


    public override bool Validate(Message msg)
    {
        return _trainNumberRegex.IsMatch(msg.Text);
    }
}