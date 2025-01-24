using System;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class InventoryManager : Node3D
    {
        [Export]
        public Player player;


        [Export]
        public ToolsManager toolsManager;
        [Export]
        public InventoryContainer InventoryContainer;

        [Export]
        public InventoryContainer ToolsInventoryContainer;

        public bool isLookingInventory = false;
        public bool GetIsLookingInventory() { return isLookingInventory; }

        public const string TmpItemResPath = "user://tmp/stack.tres";

        public override void _Ready()
        {
            player.model.ObserveLockToCamera += GetIsLookingInventory;
        }
        public override void _Process(double delta)
        {

        }
        public override void _ExitTree()
        {
            player.model.ObserveLockToCamera -= GetIsLookingInventory;
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
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void RecieveOpenInventory()
        {
            toolsManager.SetTool(null);
            isLookingInventory = true;

            player.model.SetHandsContinousState("look");

            InventoryContainer.Visible = true;
            ToolsInventoryContainer.Visible = true;
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void ServerCloseInventory()
        {
            Rpc(MethodName.RecieveCloseInventory);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void RecieveCloseInventory()
        {
            isLookingInventory = false;

            InventoryContainer.Visible = false;
            ToolsInventoryContainer.Visible = false;

            toolsManager.SetupCurrentItem();
        }
        #endregion
    }
}