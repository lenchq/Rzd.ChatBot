﻿using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class EndOfFormsMessage : MessageDialogue
{
    public EndOfFormsMessage() : base("endOfForms")
    {
    }

    public override State State => State.EndOfFormsRedir;
    public override State NextState(Context ctx) => State.Menu;
}