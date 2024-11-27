using System;
using Godot;

namespace Game
{
    public partial class InventoryItemsRenderer : Node3D
    {
        [Export]
        public InventoryManager inventoryManager;
        [Export]
        public PackedScene StackRendererScene;

        public override void _Ready()
        {
            inventoryManager.storage.OnUpdate += RenderItems;
        }

        public void RenderItems()
        {
            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }

            int i = 0;
            foreach (var itemStack in inventoryManager.storage.ItemsStacks)
            {
                var stackInstance = StackRendererScene.Instantiate<InventoryItemStackRenderer>();
                AddChild(stackInstance);
                stackInstance.Position = new Vector3(0.0f, 0.0f, i * 0.5f);
                stackInstance.Setup(itemStack, this);
                i += 1;
            }
        }
    }
}