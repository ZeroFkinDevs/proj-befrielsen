using System;
using System.Diagnostics;
using Game.Utils;
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

        public void InstantiateGrab(PackedScene propScene)
        {

        }

        #region GrabProp
        public void RequestGrabProp(Prop prop, Transform3D? targetPointTransform = null)
        {
            var targetPointTransformVariant = targetPointTransform.HasValue ? targetPointTransform.Value : prop.GlobalTransform;
            RpcId(1, MethodName.ServerGrabProp, prop.GlobalTransform, targetPointTransformVariant, prop.GetMultiplayerPath());
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerGrabProp(Transform3D transform, Transform3D targetPointTransform, NodePath propPath)
        {
            Rpc(MethodName.RecieveGrabProp, transform, targetPointTransform, propPath);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveGrabProp(Transform3D transform, Transform3D targetPointTransform, NodePath propPath)
        {
            if (GrabbingProp != null && IsGrabbing)
            {
                RecieveUngrabProp(0.0f);
            }
            var prop = this.GetMultiplayerNode<Prop>(propPath);

            prop.GlobalTransform = transform;
            GrabbingProp = prop;

            connector.Target.GlobalTransform = targetPointTransform;
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
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerUngrabProp(float force)
        {
            Rpc(MethodName.RecieveUngrabProp, force);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveUngrabProp(float force)
        {
            IsGrabbing = false;
            connector.Body = null;

            if (IsInstanceValid(GrabbingProp))
            {
                GrabbingProp.GrabEnd();
                GrabbingProp.CanRequestImpulses = true;
                if (player.Controllable)
                {
                    Vector3 velocity = -GlobalTransform.Basis.Z * force;
                    velocity += GrabbingProp.LinearVelocity;
                    GrabbingProp.RequestImpulse(velocity, GrabbingProp.GlobalTransform);
                }
            }

            GrabbingProp = null;
        }
        #endregion

        public override void _Process(double delta)
        {
            if (!player.Controllable) return;

            if (IsGrabbing)
            {
                if (!IsInstanceValid(GrabbingProp))
                {
                    RequestUngrabProp(0.0f);
                }
                if (GrabbingProp != null && player.ControlGroup == Player.ControlGroupEnum.WORLD)
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

        public void GrabPropInstance(Node node)
        {
            Prop prop = null;
            var targetPos = player.Camera.GlobalPosition - player.Camera.GlobalTransform.Basis.Z;

            foreach (var child in node.GetChildren())
            {
                if (child is Prop childprop)
                {
                    prop = childprop;
                    break;
                }
            }
            if (prop != null)
            {
                if (node is Node3D node3d)
                {
                    node3d.GlobalPosition = player.Camera.GlobalPosition;
                }
                prop.ModelSmoothConnector.NoSmooth = true;
                if (player.Controllable)
                {
                    var targetTrans = prop.GlobalTransform;
                    targetTrans.Origin = targetPos;
                    prop.RequestImpulse(-player.Camera.GlobalTransform.Basis.Z, prop.GlobalTransform);
                    RequestGrabProp(prop, targetTrans);
                }
            }
        }
    }
}