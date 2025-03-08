using Godot;
using System;

namespace Game
{
    public partial class BoneToBone : Node3D
    {
        [Export]
        public Node3D animationControllerOrUserA;
        public AnimationController animControllerA
        {
            get
            {
                if (animationControllerOrUserA is IAnimationControllerUser user) return user.animationController;
                if (animationControllerOrUserA is AnimationController controller) return controller;
                return null;
            }
        }
        [Export]
        public string boneNameA;
        public int boneAId = -1;

        [Export]
        public Node3D animationControllerOrUserB;
        public AnimationController animControllerB
        {
            get
            {
                if (animationControllerOrUserB is IAnimationControllerUser user) return user.animationController;
                if (animationControllerOrUserB is AnimationController controller) return controller;
                return null;
            }
        }
        [Export]
        public string boneNameB;
        public int boneBId = -1;

        [Export]
        public bool syncRotation;
        [Export]
        public bool syncPosition;

        public override void _Process(double delta)
        {
            if (!Visible) return;
            if (animControllerA == null) return;
            if (animControllerB == null) return;
            if (boneAId == -1) boneAId = animControllerA.skeleton3D.FindBone(boneNameA);
            if (boneBId == -1) boneBId = animControllerB.skeleton3D.FindBone(boneNameB);

            var transformA = animControllerA.GetBoneGlobalPose(boneAId);
            var transformB = animControllerB.GetBoneGlobalPose(boneBId);
            if (syncRotation) transformA.Basis = transformB.Basis;
            if (syncPosition) transformA.Origin = transformB.Origin;
            animControllerA.skeleton3D.SetBoneGlobalPoseOverride(boneAId, animControllerA.skeleton3D.GlobalTransform.AffineInverse() * transformA, 1.0f, true);
        }
        public override void _PhysicsProcess(double delta)
        {

        }
    }
}