using System;
using Godot;

namespace Game
{
    public partial class InventoryManager : Node3D
    {
        [Export]
        public Player player;

        [Export]
        public InventoryContainer InventoryContainer;

        [Export]
        public InventoryContainer ToolsInventoryContainer;

        public const string TmpItemResPath = "user://tmp/stack.tres";

        public override void _Ready()
        {

        }

        public override void _Process(double delta)
        {
            PositionInventory(InventoryContainer, player.model.LeftHandBoneID);
            PositionInventory(ToolsInventoryContainer, player.model.RightHandBoneID);
        }

        public void PositionInventory(InventoryContainer container, int boneID)
        {
            var trans = player.model.GetBoneGlobalPose(boneID);
            if (container != null)
            {
                container.GlobalTransform = trans;
            }
        }

        #region open-close inventory
        public void OpenInventory()
        {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
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

            InventoryContainer.Visible = true;
            ToolsInventoryContainer.Visible = true;
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

            InventoryContainer.Visible = false;
            ToolsInventoryContainer.Visible = false;
        }
        #endregion
    }
}