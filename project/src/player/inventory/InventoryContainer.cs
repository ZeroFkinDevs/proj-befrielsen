using System;
using Godot;

namespace Game
{
    public partial class InventoryContainer : Node3D
    {
        [Export]
        public InventoryManager inventoryManager;

        [Export]
        public ItemsStorage storage;
        [Export]
        public string boneName;
        private int boneId;

        public override void _EnterTree()
        {
            storage = new ItemsStorage();
        }
        public override void _Ready()
        {
            if (inventoryManager.player.Controllable)
            {
                BroadcastItemStacks();
            }
            boneId = inventoryManager.player.model.skeleton3D.FindBone(boneName);
        }
        public override void _Process(double delta)
        {
            var trans = inventoryManager.player.model.GetBoneGlobalPose(boneId);
            GlobalTransform = trans;
        }
        public void BroadcastItemStacks()
        {
            inventoryManager.player.tmpStorage.BroadcastArrayOfResources(storage.ItemsStacks, "stack", this, MethodName.RecieveItems, null);
        }

        public void AddItemStacks(Godot.Collections.Array<ItemStack> stacks)
        {
            storage.AddItemStacks(stacks);
            BroadcastItemStacks();
        }
        public void RemoveItemStack(ItemStack stack)
        {
            storage.RemoveItemStack(stack);
            BroadcastItemStacks();
        }
        public void ConsumeItem(ItemResource itemResource, int amount)
        {
            if (storage.ConsumeItem(itemResource, amount))
            {
                BroadcastItemStacks();
            }
        }
        public void RecieveItems(Godot.Collections.Array<ItemStack> stacks, Godot.Collections.Array<string> args = null)
        {
            storage.SetItemsStacks(stacks);
        }

    }
}