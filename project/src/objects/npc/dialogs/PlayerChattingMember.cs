using System;
using System.Collections.Generic;
using Godot;

namespace Game.Dialog
{
    public partial class PlayerChattingMember : ChattingMember
    {
        [Export]
        public Player player;

        public override void _EnterTree()
        {
            base._EnterTree();
            ChattingMembers.OnAdded += OnMemberAdded;
            ChattingMembers.OnRemoved += OnMemberRemoved;
        }
        protected override void SetupConversation()
        {
            base.SetupConversation();
        }

        private void OnMemberAdded(IChattingMember member)
        {
            if (member is ISubtitlesProvider subtitlesProvider && player.Current)
            {
                subtitlesProvider.subtitlesUnit.TargetSubscription += GetSubtitlesViewTarget;
            }
        }
        private void OnMemberRemoved(IChattingMember member)
        {
            if (member is ISubtitlesProvider subtitlesProvider && player.Current)
            {
                subtitlesProvider.subtitlesUnit.TargetSubscription -= GetSubtitlesViewTarget;
            }
        }
        private Node3D GetSubtitlesViewTarget()
        {
            return player.Camera;
        }
    }
}