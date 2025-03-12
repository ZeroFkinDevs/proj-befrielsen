using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public interface IChattingNode
    {
        public void Execute();
        public IChattingNode Fetch(string code);
    }
    public class ChattingNode : IChattingNode
    {
        public string Code { get; }
        private Func<bool> condition;
        private Action characterAction;

        public ChattingNode(string code, Action charAction)
        {
            Code = code;
            characterAction = charAction;
        }

        public ChattingNode WithCondition(Func<bool> condition)
        {
            this.condition = condition;
            return this;
        }

        public IChattingNode Fetch(string code)
        {
            if (Code != code) return null;
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