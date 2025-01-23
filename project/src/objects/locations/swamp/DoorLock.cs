using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class DoorLock : Node3D, IInteractable
    {
        [Export]
        public bool Opened = false;
        public InteractionTypeEnum InteractionType => !Opened ? InteractionTypeEnum.TOUCH : InteractionTypeEnum.NONE;
        [Export]
        public string InteractMessage = "открыть люк";
        public string InteractableDescription => InteractMessage;

        [Export]
        public HookesConnector Connector;
        [Export]
        public Node3D TargetToSet;

        public void Interact(IUser user)
        {
            RpcId(1, MethodName.ServerInteract);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerInteract()
        {
            Rpc(MethodName.InteractRecieve);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void InteractRecieve()
        {
            Opened = true;
            Connector.Target = TargetToSet;
        }
    }
}