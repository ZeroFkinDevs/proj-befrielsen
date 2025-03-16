using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public class ChattingResponse : IChattingNode
    {
        public string Code { get; }
        public string Text { get; }
        private Func<bool> condition;
        private Action characterAction;

        public ChattingResponse(string code, string text, Action charAction)
        {
            Code = code;
            Text = text;
            characterAction = charAction;
        }

        public ChattingResponse WithCondition(Func<bool> condition)
        {
            this.condition = condition;
            return this;
        }

        public IChattingNode Fetch(string code = "")
        {
            if (condition == null) return this;
            if (!condition.Invoke()) return null;
            return this;
        }

        public void Execute()
        {
            characterAction();
        }
    }
}