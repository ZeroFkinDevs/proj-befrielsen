using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public class ChattingCollectionNode : IChattingNode
    {

        protected List<IChattingNode> ChattingNodes { get; set; }
        public ChattingCollectionNode()
        {
            ChattingNodes = new List<IChattingNode>();
        }

        public void AddNode(IChattingNode node)
        {
            ChattingNodes.Add(node);
        }

        public void Execute()
        {
            return;
        }

        public IChattingNode Fetch(string code)
        {
            foreach (var node in ChattingNodes)
            {
                var match = node.Fetch(code);
                if (match != null) return match;
            }
            return null;
        }
    }
}