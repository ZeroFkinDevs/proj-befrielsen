using Godot;
using Godot.Collections;

namespace Game
{
    public partial class EyeHookerAnimationController : AnimationController
    {
        public Vector3 LookingTarget;
        public int rotatorBoneId;

        public override void _Ready()
        {
            base._Ready();
            rotatorBoneId = skeleton3D.FindBone("mouth_rotator");
            SetState("state", "idle");
        }

        public void Wake()
        {
            SetState("wake_state", "wake");
        }
        public void Idle()
        {
            SetState("state", "idle");
            SetState("wake_state", "wake");
        }
        public void Take()
        {
            SetState("take_state", "take");
            SetState("state", "holding");
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            // look update
            var trans = GetBoneGlobalPose(rotatorBoneId);
            trans.Basis = new Basis(new Quaternion(trans.Basis).Normalized()).Slerp(new Basis(new Quaternion(trans.LookingAt(LookingTarget).Basis).Normalized()), 0.05f);
            skeleton3D.SetBoneGlobalPoseOverride(rotatorBoneId, skeleton3D.GlobalTransform.AffineInverse() * trans, 1.0f, true);
        }
    }
}