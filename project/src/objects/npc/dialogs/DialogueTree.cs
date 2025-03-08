using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public partial class DialogueTree : Node3D
    {
        public NpcCharacterController npc;
        private List<IDialogNode> nodes = new();
        private IDialogNode _currentDialogNode = null;
        public IDialogNode CurrentDialogNode { get => _currentDialogNode; }

        public override void _Ready()
        {

        }

        public void SetupNodes()
        {
            nodes.Add(new DialogueNode("_", () => Say("Привет, ты здесь один?"))
                .WithResponses(
                    ("Что ты здесь делаешь?", () => Goto("why_are_you_here")),
                    ("Что это за место?", () => Goto("whats_that_place"))
                ));

            nodes.Add(new DialogueNode("why_are_you_here", () => Say("Я здесь давно, и нашла это место случайно.")));
            nodes.Add(new DialogueNode("whats_that_place", () => Say("Это старое убежище, но здесь безопасно.")));

            nodes.Reverse();
        }

        public void Say(string text)
        {
            GD.Print($"Шерил: {text}");
        }

        public IDialogNode FindDialog(string code)
        {
            foreach (var node in nodes)
            {
                var match = node.GetMatch(code);
                if (match != null) return match;
            }
            return null;
        }

        public void Goto(string nodeCode)
        {
            var dialogNode = FindDialog(nodeCode);
            if (dialogNode != null)
            {
                _currentDialogNode = dialogNode;
                dialogNode.Execute();
            }
        }
    }
}