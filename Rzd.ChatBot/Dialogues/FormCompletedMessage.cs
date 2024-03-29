﻿using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormCompletedMessage : MessageDialogue
{
    public FormCompletedMessage() : base("formCompleted")
    {
    }

    public override State State => State.FormCompleted;
    public override State NextState(Context ctx) => State.MyForm;
}