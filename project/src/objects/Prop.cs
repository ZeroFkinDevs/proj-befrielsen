using Godot;
using Godot.Collections;

namespace Game
{
    public partial class Prop : RigidBody3D, IInteractable
    {
        class PhysicsPackStruct
        {
            public PhysicsPackStruct(Transform3D transform, Vector3 linearVelocity, Vector3 angularVelocity)
            {
                Transform = transform;
                LinearVelocity = linearVelocity;
                AngularVelocity = angularVelocity;
            }
            public Transform3D Transform;
            public Vector3 LinearVelocity;
            public Vector3 AngularVelocity;
        }
        private PhysicsPackStruct physicsToRecieve = null;
        private int shouldSharePhysics = 0;
        public bool CanRequestImpulses = true;

        public InteractionTypeEnum InteractionType => InteractionTypeEnum.GRAB;

        public void RequestImpulse(Vector3 velocity, Transform3D? transform)
        {
            if (!CanRequestImpulses) return;

            if (transform.HasValue)
                RpcId(1, MethodName.ServerPositionedImpulse, velocity, transform.Value);
            else
                RpcId(1, MethodName.ServerImpulse, velocity);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerImpulse(Vector3 velocity)
        {
            ApplyCentralImpulse(velocity);
            shouldSharePhysics = 1;
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerPositionedImpulse(Vector3 velocity, Transform3D transform)
        {
            GlobalTransform = transform;
            ApplyCentralImpulse(velocity);
            shouldSharePhysics = 1;
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecievePhysics(Transform3D transform, Vector3 linearVelocity, Vector3 angularVelocity)
        {
            var pack = new PhysicsPackStruct(transform, linearVelocity, angularVelocity);
            physicsToRecieve = pack;
        }

        public override void _PhysicsProcess(double delta)
        {
            DelayedSharePhysics();

            if (physicsToRecieve != null)
            {
                GlobalTransform = physicsToRecieve.Transform;
                LinearVelocity = physicsToRecieve.LinearVelocity;
                AngularVelocity = physicsToRecieve.AngularVelocity;
                physicsToRecieve = null;
            }
        }

        public void DelayedSharePhysics()
        {
            if (shouldSharePhysics == 1)
            {
                shouldSharePhysics += 1;
            }
            else
            if (shouldSharePhysics == 2)
            {
                Rpc(MethodName.RecievePhysics, GlobalTransform, LinearVelocity, AngularVelocity);
                shouldSharePhysics = 0;
            }
        }

        public void Interact(IUser user)
        {

        }
    }
}