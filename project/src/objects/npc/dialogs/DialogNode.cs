using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public interface IDialogNode
    {
        public void Execute();
        public IDialogNode GetMatch(string code);
    }
    public class DialogueNode : IDialogNode
    {
        public string Code { get; }
        private List<(string option, Action action)> responses = new();
        public List<(string option, Action action)> Responses { get => responses; }
        private Func<bool> condition;
        private Action characterAction;

        public DialogueNode(string code, Action charAction)
        {
            Code = code;
            characterAction = charAction;
        }

        public DialogueNode WithCondition(Func<bool> condition)
        {
            this.condition = condition;
            return this;
        }

        public DialogueNode WithResponses(params (string option, Action action)[] responses)
        {
            this.responses.AddRange(responses);
            return this;
        }

        public IDialogNode GetMatch(string code)
        {
            if (Code != code) return null;
            if (condition == null) return this;
            if (!condition.Invoke()) return null;
            return this;
        }

        public void Execute()
        {
            characterAction();
            foreach (var (option, action) in responses)
            {
                GD.Print($"- {option}");
            }
        }
    }
}