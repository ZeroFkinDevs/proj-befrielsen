using Godot;
using System;

namespace Game
{
    public partial class NodeToBoneConnector : Node3D
    {
        [Export]
        public Node3D node;
        [Export]
        public Skeleton3D skeleton;
        [Export]
        public string boneName;

        int boneId = -1;

        public override void _Ready()
        {

        }

        public override void _Process(double delta)
        {
            Sync();
        }
        public void Sync()
        {
            if (skeleton == null) return;
            if (boneId == -1) boneId = skeleton.FindBone(boneName);
            if (node == null) return;

            var pose = skeleton.GlobalTransform * skeleton.GetBoneGlobalPose(boneId);
            node.GlobalTransform = pose;
        }
        public override void _PhysicsProcess(double delta)
        {

        }
    }
}