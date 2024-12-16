using System;
using Godot;

namespace Game
{
    public partial class InventoryItemsRenderer : Node3D
    {
        [Export]
        public InventoryContainer inventoryContainer;
        [Export]
        public PackedScene StackRendererScene;

        public override void _Ready()
        {
            inventoryContainer.storage.OnUpdate += RenderItems;
        }

        public void RenderItems()
        {
            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }

            int i = 0;

            foreach (var itemStack in inventoryContainer.storage.ItemsStacks)
            {
                var stackInstance = StackRendererScene.Instantiate<InventoryItemStackRenderer>();
                AddChild(stackInstance);
                stackInstance.Position = new Vector3(0.0f, 0.0f, i * 0.05f);
                stackInstance.Setup(itemStack, this);
                i += 1;
            }
        }
    }
}