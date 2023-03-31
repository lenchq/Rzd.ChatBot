using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class ConfirmQrData : ActionDialogue
{
    public ConfirmQrData() : base("confirmQr")
    {
        Options = new BotAction[] { Correct, Incorrect };
    }

    //TODO REDIRECT
    [OptionIndex(0)]
    private ValueTask<State> Correct(Context ctx)
    {
        if (ctx.UserContext.StartData is not null)
        {
            var trainData = ParseTrainData(ctx.UserContext.StartData);
            ctx.UserForm.TrainNumber = trainData.TrainNumber;
            ctx.UserForm.Seat = trainData.Seat;

            ctx.UserContext.StartData = null;
        }
        return new(State.PermissionsInput);
    }

    [OptionIndex(1)]
    private ValueTask<State> Incorrect(Context ctx)
    {
        if (ctx.UserContext.StartData is not null)
        {
            ctx.UserContext.StartData = null;
        }
        return new(State.ScanQr);
    }

    public override State State => State.ConfirmQr;

    public override string GetText(Context ctx)
    {
        if (ctx.UserContext.StartData is not null)
        {
            var data = ParseTrainData(ctx.UserContext.StartData);

            return LocalizationWrapper.FormattedText("caption", new {data.TrainNumber, data.Seat});
        }
        return LocalizationWrapper.FormattedText("caption", ctx.UserForm);
    }

    private (string TrainNumber, int Seat) ParseTrainData(string data)
    {
        var regex =  ScanQr.TrainDataRegex.Match(data);
        var trainNumGroup = regex.Groups[1];
        var trainNum = trainNumGroup.Value.ToUpper();
        var trainSeat = int.Parse( regex.Groups[2].Value );
        return (trainNum, trainSeat);
    }
}