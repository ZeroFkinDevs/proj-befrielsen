using System;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class ObjectGrabber : Node3D
    {
        [Export]
        public HookesConnector connector;
        [Export]
        public Player player;

        public Prop GrabbingProp;
        public bool IsGrabbing;
        public void Grab(IInteractable interactable)
        {
            if (IsGrabbing) return;

            if (interactable is Prop prop)
            {
                GrabProp(prop);
            }
        }

        public void GrabProp(Prop prop)
        {
            GrabbingProp = prop;

            connector.Target.GlobalTransform = prop.GlobalTransform;
            connector.Body = prop;
            prop.CanRequestImpulses = false;

            IsGrabbing = true;
        }
        public void UngrabProp(float force)
        {
            if (GrabbingProp == null) return;
            IsGrabbing = false;
            connector.Body = null;
            GrabbingProp.CanRequestImpulses = true;
            Vector3 velocity = -GlobalTransform.Basis.Z * force;
            velocity += GrabbingProp.LinearVelocity;
            GrabbingProp.RequestImpulse(velocity, GrabbingProp.GlobalTransform);
            GrabbingProp = null;
        }

        public override void _Process(double delta)
        {
            if (!player.Controllable) return;

            if (IsGrabbing)
            {
                if (GrabbingProp != null)
                {
                    if (Input.IsActionJustPressed("alt_fire"))
                    {
                        UngrabProp(0.0f);
                    }
                    else if (Input.IsActionJustPressed("fire"))
                    {
                        UngrabProp(10.0f);
                    }
                }
            }
        }
    }
}