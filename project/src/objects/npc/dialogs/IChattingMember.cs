using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public interface IChattingMember
    {
        public string Code { get; }
        public string Name { get; }
        public ChattingResponsesCollection ResponsesCollection { get; }
    }
}