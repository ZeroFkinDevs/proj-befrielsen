using Godot;
using System;

namespace Game
{
    public partial class BoneConnector : Node3D
    {
        [Export]
        public Node3D targetNode;
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
            if (skeleton == null) return;
            if (boneId == -1) boneId = skeleton.FindBone(boneName);

            var transform = targetNode.GlobalTransform;
            skeleton.SetBoneGlobalPoseOverride(boneId, skeleton.GlobalTransform.AffineInverse() * transform, 1.0f, true);
        }
        public override void _PhysicsProcess(double delta)
        {

        }
    }
}