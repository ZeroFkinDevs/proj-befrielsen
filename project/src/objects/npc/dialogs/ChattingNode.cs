using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private Func<Task> characterAction;

        public ChattingNode(string code)
        {
            Code = code;
        }

        public ChattingNode WithAction(Func<Task> charAction)
        {
            characterAction = charAction;
            return this;
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

        public async void Execute()
        {
            if (characterAction != null) await characterAction();
        }
    }
}