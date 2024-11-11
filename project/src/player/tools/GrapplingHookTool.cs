using System;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class GrapplingHookTool : Node3D
    {
        [Export]
        public Player player;
        [Export]
        public Skeleton3D Skeleton;
        public bool Grabbed = false;
        public Vector3 GrabPoint;
        public Vector3 GrabNormal;
        public Vector3 GrabUpVector;
        public float hookPosition = 0.0f;

        // private int 
        public void SetBoneGlobalPose(string boneName, Transform3D transform)
        {
            var idx = Skeleton.FindBone(boneName);
            Skeleton.SetBoneGlobalPoseOverride(idx, Skeleton.GlobalTransform.AffineInverse() * transform, hookPosition, true);
        }
        public void SetBonePose(string boneName, Transform3D transform)
        {
            var idx = Skeleton.FindBone(boneName);
        }
        public void Grab()
        {
            var space = GetWorld3D().DirectSpaceState;
            var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, GlobalPosition - GlobalTransform.Basis.Z * 100.0f);
            var result = space.IntersectRay(query);
            if (result.Count > 0)
            {
                GrabPoint = result.Get<Vector3>("position");
                GrabNormal = result.Get<Vector3>("normal");
                GrabUpVector = GlobalTransform.Basis.Y;
                Grabbed = true;
            }
        }
        public void UnGrab()
        {
            Grabbed = false;
        }
        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("fire"))
            {
                Grab();
            }
            if (Input.IsActionJustPressed("alt_fire"))
            {
                UnGrab();
            }

            if (Grabbed)
            {
                hookPosition = Mathf.Lerp(hookPosition, 1.0f, (float)delta * 10.0f);
                var vel = player.Velocity;
                var dist = GrabPoint - player.GlobalPosition;
                var dir = dist.Normalized();
                var length = dist.Length();
                vel += (dir * length) * 0.1f;
                vel.Y *= 0.98f;
                player.Velocity = vel;
            }
            else
            {
                hookPosition = Mathf.Lerp(hookPosition, 0.0f, (float)delta * 10.0f);
            }
            var trans = Transform3D.Identity;
            trans.Origin = GrabPoint + (GrabNormal * 0.6f);

            if (GrabNormal != Vector3.Zero)
            {
                trans = trans.LookingAt(trans.Origin - GrabNormal, GrabUpVector);
            }
            SetBoneGlobalPose("hook", trans);
            SetBoneGlobalPose("rope_end", trans);
        }
    }
}