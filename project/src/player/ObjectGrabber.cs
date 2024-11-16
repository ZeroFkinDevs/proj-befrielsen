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
                RequestGrabProp(prop);
            }
        }
        #region GrabProp
        public void RequestGrabProp(Prop prop)
        {
            RpcId(1, MethodName.ServerGrabProp, prop.GlobalTransform, prop.GetPath());
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerGrabProp(Transform3D transform, NodePath propPath)
        {
            Rpc(MethodName.RecieveGrabProp, transform, propPath);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveGrabProp(Transform3D transform, NodePath propPath)
        {
            var prop = GetNode<Prop>(propPath);
            prop.GlobalTransform = transform;
            GrabbingProp = prop;

            connector.Target.GlobalTransform = prop.GlobalTransform;
            connector.Body = prop;
            prop.CanRequestImpulses = false;
            GrabbingProp.GrabStart();

            IsGrabbing = true;
        }
        #endregion
        #region UngrabProp
        public void RequestUngrabProp(float force)
        {
            RpcId(1, MethodName.ServerUngrabProp, force);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerUngrabProp(float force)
        {
            Rpc(MethodName.RecieveUngrabProp, force);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveUngrabProp(float force)
        {
            IsGrabbing = false;
            connector.Body = null;

            if (GrabbingProp == null) return;
            GrabbingProp.GrabEnd();
            GrabbingProp.CanRequestImpulses = true;

            if (player.Controllable)
            {
                Vector3 velocity = -GlobalTransform.Basis.Z * force;
                velocity += GrabbingProp.LinearVelocity;
                GrabbingProp.RequestImpulse(velocity, GrabbingProp.GlobalTransform);
            }

            GrabbingProp = null;
        }

        #endregion

        public override void _Process(double delta)
        {
            if (!player.Controllable) return;

            if (IsGrabbing)
            {
                if (GrabbingProp != null)
                {
                    if (Input.IsActionJustPressed("alt_fire"))
                    {
                        RequestUngrabProp(0.0f);
                    }
                    else if (Input.IsActionJustPressed("fire"))
                    {
                        RequestUngrabProp(10.0f);
                    }
                }
            }
        }
    }
}