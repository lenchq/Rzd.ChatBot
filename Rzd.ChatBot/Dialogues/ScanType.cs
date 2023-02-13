// using Rzd.ChatBot.Extensions;
// using Rzd.ChatBot.Localization;
// using Rzd.ChatBot.Model;
// using Rzd.ChatBot.Repository;
// using Rzd.ChatBot.Types;
// using Rzd.ChatBot.Types.Enums;
//
//
// namespace Rzd.ChatBot.Dialogues;
//
// public sealed class ScanTypeDialogue : ActionDialogue
// {
//     public override State State => State.SelectScanType;
//
//     public ScanTypeDialogue()
//         : base("scanType")
//     {
//         Options = new BotAction[] {ScanTypePhoto, ScanTypeText};
//     }
//
//
//     public override void WrongAnswer(Context ctx)
//     {
//         //TODO
//     }
//
//     public State ScanTypePhoto(Context ctx)
//         => State.ScanQr;
//
//     public State ScanTypeText(Context ctx)
//         => State.AddTrainNumber;
// }