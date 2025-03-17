using System;
using System.Collections.Generic;
using System.Security;
using Godot;

namespace Game.Dialog
{
    public class ChattingResponse : IChattingNode
    {
        public string Code { get; }
        public string Text { get; }
        private Func<string> disability;
        private Action characterAction;
        private bool used = false;

        public ChattingResponse(string code, string text, Action charAction)
        {
            Code = code;
            Text = text;
            characterAction = charAction;
        }

        public ChattingResponse WithDisability(Func<string> disability)
        {
            this.disability = disability;
            return this;
        }

        public IChattingNode Fetch(string code = "")
        {
            if (disability == null) return this;
            return this;
        }

        public (bool, string) CheckAbility()
        {
            if (disability == null) return (true, "");
            if (used) return (false, "использовано");

            var message = disability.Invoke();
            if (message != null) return (false, message);

            return (true, "");
        }

        public void Execute()
        {
            characterAction();
            used = true;
        }
    }
}