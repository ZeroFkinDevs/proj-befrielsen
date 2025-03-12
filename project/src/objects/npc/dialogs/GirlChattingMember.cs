using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public partial class GirlChattingMember : ChattingMember
    {
        public override void _Ready()
        {
            base._Ready();
            ChattingMembers.OnAdded += OnMemberAdded;
        }
        protected override void SetupConversation()
        {
            base.SetupConversation();

            NodesCollection.AddNode(new ChattingNode("_greet", () =>
            {
                ChattingMembers.ForEachMember((member) =>
                {
                    member.ResponsesCollection.SetForMember(this, new List<ChattingResponse>{
                        new ChattingResponse("hi_girl", "Hi girl", ()=>{TryExecuteNode("hi_girl");}),
                        new ChattingResponse("hi_ugly_shit", "Hi ugly shit", ()=>{TryExecuteNode("hi_ugly_shit");}),
                    });
                });
            }));

            NodesCollection.AddNode(new ChattingNode("hi_ugly_shit", () =>
            {
                GD.Print("um, ugh...");
            }));
            NodesCollection.AddNode(new ChattingNode("hi_girl", () =>
            {
                GD.Print("oh, you make me shy, hi!");
            }));
        }

        private void OnMemberAdded(IChattingMember member)
        {
            TryExecuteNode("_greet");
        }
    }
}