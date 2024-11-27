using System;
using Godot;

namespace Game
{
    public partial class InventoryItemStackRenderer : Node3D
    {
        public ItemStack itemStack;
        public Node3D itemInstance;
        public InventoryItemsRenderer itemsRenderer;

        public void Setup(ItemStack stack, InventoryItemsRenderer renderer)
        {
            itemStack = stack;
            itemsRenderer = renderer;

            itemInstance = itemStack.ItemRes.MeshScene.Instantiate<Node3D>();
            AddChild(itemInstance);
            if (itemInstance.GetChild<Node3D>(0) is VisualInstance3D visInstance)
            {
                var aabb = visInstance.GetAabb();
                var size = aabb.Size;
                var scale = 0.1f / size[(int)size.MaxAxisIndex()];
                itemInstance.Scale = Vector3.One * scale;
            }
        }
    }
}