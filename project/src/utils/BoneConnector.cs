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

        int boneId;

        public override void _Ready()
        {
            boneId = skeleton.FindBone(boneName);
        }

        public override void _Process(double delta)
        {
            var transform = targetNode.GlobalTransform;
            skeleton.SetBoneGlobalPoseOverride(boneId, skeleton.GlobalTransform.AffineInverse() * transform, 1.0f, true);
        }
        public override void _PhysicsProcess(double delta)
        {

        }
    }
}