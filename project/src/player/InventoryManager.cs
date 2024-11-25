using System;
using Godot;

namespace Game
{
    public partial class InventoryManager : Node3D
    {
        [Export]
        Player player;

        public void OpenInventory()
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            RpcId(1, MethodName.ServerOpenInventory);
        }
        public void CloseInventory()
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            RpcId(1, MethodName.ServerCloseInventory);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void ServerOpenInventory()
        {
            Rpc(MethodName.RecieveOpenInventory);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void RecieveOpenInventory()
        {
            player.model.LockToCamera = true;
            player.model.SetHandsContinousState("look");
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void ServerCloseInventory()
        {
            Rpc(MethodName.RecieveCloseInventory);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void RecieveCloseInventory()
        {
            player.model.LockToCamera = false;
        }
    }
}