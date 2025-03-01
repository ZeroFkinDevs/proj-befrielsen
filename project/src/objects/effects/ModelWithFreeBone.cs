using System;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class ModelWithFreeBone : Node3D, ITeleportableTransform
    {
        [Export]
        public string BoneName;
        [Export]
        public Vector3 TargetOffset;
        Skeleton3D skeleton;
        int boneId;
        Transform3D globalPose;
        Vector3 velocity;
        Vector3 angularVelocity;

        public override void _Ready()
        {
            foreach (var node in this.GetChildrenRecursively())
            {
                if (node is Skeleton3D foundSkeleton)
                {
                    skeleton = foundSkeleton;
                    boneId = skeleton.FindBone(BoneName);
                    globalPose = skeleton.GlobalTransform * skeleton.GetBoneGlobalPose(boneId);
                    break;
                }
            }
            CallDeferred(MethodName.DefferedReady);
        }
        public void DefferedReady()
        {
            ResetPose();
        }
        public override void _Process(double delta)
        {
            Vector3 positionDifference = (GlobalTransform.Origin + TargetOffset) - globalPose.Origin;
            Basis rotationDifference = GlobalTransform.Basis * globalPose.Basis.Inverse();

            float linearSpringStiffness = 300f;
            float linearSpringDamping = 20f;
            float maxLinearForce = 40f;
            float angularSpringStiffness = 200f;
            float angularSpringDamping = 5f;
            float maxAngularForce = 400f;

            Vector3 force = HookesConnector.HookesLaw(positionDifference, velocity, linearSpringStiffness, linearSpringDamping);
            force = force.LimitLength(maxLinearForce);
            velocity += force * (float)delta;
            velocity += Vector3.Down * 10 * (float)delta;
            globalPose.Origin += velocity * (float)delta;

            Vector3 torque = HookesConnector.HookesLaw(rotationDifference.GetEuler(), angularVelocity, angularSpringStiffness, angularSpringDamping);
            torque = torque.LimitLength(maxAngularForce);

            angularVelocity += torque * (float)delta;

            globalPose.Basis = globalPose.Basis.Rotated(Vector3.Right, (angularVelocity * (float)delta).X);
            globalPose.Basis = globalPose.Basis.Rotated(Vector3.Up, (angularVelocity * (float)delta).Y);
            globalPose.Basis = globalPose.Basis.Rotated(Vector3.Back, (angularVelocity * (float)delta).Z);

            skeleton.SetBoneGlobalPoseOverride(boneId, skeleton.GlobalTransform.AffineInverse() * globalPose, 1.0f, true);
        }

        public void ResetPose()
        {
            globalPose.Origin = GlobalTransform.Origin + TargetOffset;
            velocity = Vector3.Zero;
            angularVelocity = Vector3.Zero;
        }

        public void RecieveTransformTeleportation(Func<Transform3D, Transform3D> teleportTransform)
        {
            globalPose = teleportTransform(globalPose);
        }
    }
}