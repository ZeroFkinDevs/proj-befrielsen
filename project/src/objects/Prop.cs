using Godot;
using Godot.Collections;

namespace Game
{
    // наверное важно не использовать для пропов MultiplayerSpawner,
    // как минимум потому что они при создании будут синхронизировать позицию для всех клиентов
    // не проблема, но все же
    public partial class Prop : RigidBody3D, IInteractable, IItemsStoring
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

        public InteractionTypeEnum _interactionType = InteractionTypeEnum.GRAB;
        public InteractionTypeEnum InteractionType => _interactionType;

        [Export]
        public SmoothConnectTransform ModelSmoothConnector;

        [Export]
        public ItemsStorage itemsStorage;

        public ItemsStorage ItemsStorageRes { get { return itemsStorage; } }

        public string InteractableDescription => "prop";


        public void GrabStart()
        {
            _interactionType = InteractionTypeEnum.NONE;
            SetCollisionLayerValue(4, false);
            SetCollisionMaskValue(4, false);
            ModelSmoothConnector.NoSmooth = false;
            if (itemsStorage != null)
            {
                SetCollisionLayerValue(6, true);
                _interactionType = InteractionTypeEnum.PICKUP;
            }
        }
        public void GrabEnd()
        {
            _interactionType = InteractionTypeEnum.GRAB;
            SetCollisionLayerValue(4, true);
            SetCollisionMaskValue(4, true);
            SetCollisionLayerValue(6, false);
        }

        public void RequestImpulse(Vector3 velocity, Transform3D? transform)
        {
            if (!CanRequestImpulses) return;

            if (transform.HasValue)
                RpcId(1, MethodName.ServerPositionedImpulse, velocity, transform.Value);
            else
                RpcId(1, MethodName.ServerImpulse, velocity);
        }
        public override void _Ready()
        {
            // чтобы синхронизировать позицию когда игрок подключается и спавнятся пропы
            RequestImpulse(Vector3.Zero, null);
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
            if (InteractionType == InteractionTypeEnum.PICKUP && itemsStorage != null)
            {
                if (user is Player player)
                {
                    player.inventoryManager.InventoryContainer.AddItemStacks(itemsStorage.ItemsStacks);
                    RpcId(1, MethodName.ServerRemove);
                }
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void ServerRemove()
        {
            Rpc(MethodName.RecieveRemove);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void RecieveRemove()
        {
            GetParent().QueueFree();
        }

    }
}