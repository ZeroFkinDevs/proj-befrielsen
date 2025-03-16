using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Dialog
{
    public partial class ChattingMember : Area3D, IChattingMember
    {
        [Export]
        public string Code { get; set; }
        string IChattingMember.Name => Code;

        public ChattingResponsesCollection ResponsesCollection { get; set; }

        protected ChattingCollectionNode NodesCollection;
        public ChattingMembersCollection ChattingMembers;

        public override void _EnterTree()
        {
            ResponsesCollection = new ChattingResponsesCollection();
            NodesCollection = new ChattingCollectionNode();

            ChattingMembers = new ChattingMembersCollection();
            ChattingMembers.OnRemoved += OnMemberRemoved;
        }

        public override void _Ready()
        {
            AreaEntered += OnAreaEntered;
            AreaExited += OnAreaExited;

            SetupConversation();
        }

        protected virtual void SetupConversation()
        {

        }

        protected virtual void TryExecuteNodeDeffered(string code)
        {
            CallDeferred(MethodName.TryExecuteNode, code);
        }

        public void TryExecuteNode(string code)
        {
            var node = NodesCollection.Fetch(code);
            if (node != null)
            {
                node.Execute();
                return;
            }
            GD.Print($"node not found '{code}'");
        }

        public void ExecuteResponse(string code)
        {
            RpcId(1, MethodName.ServerExecuteResponse, code);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerExecuteResponse(string code)
        {
            Rpc(MethodName.RecieveExecuteResponse, code);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveExecuteResponse(string code)
        {
            var response = ResponsesCollection.GetByCode(code);
            if (response != null)
            {
                response.Execute();
            }
        }

        protected void OnAreaEntered(Node body)
        {
            if (body is IChattingMember member)
            {
                ChattingMembers.Add(member);
            }
        }
        protected void OnAreaExited(Node body)
        {
            if (body is IChattingMember member)
            {
                ChattingMembers.Remove(member);
            }
        }
        private void OnMemberRemoved(IChattingMember member)
        {
            ResponsesCollection.ClearForMember(member);
        }
    }
}