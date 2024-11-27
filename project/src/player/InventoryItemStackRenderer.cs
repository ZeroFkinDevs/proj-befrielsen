using System;
using Godot;

namespace Game
{
    public partial class InventoryItemStackRenderer : Area3D, IInteractable
    {
        public ItemStack itemStack;
        public Node3D itemInstance;
        public InventoryItemsRenderer itemsRenderer;
        public bool isDragging = false;
        public float cameraDistance = 0.0f;

        public InteractionTypeEnum InteractionType
        {
            get
            {
                if (itemsRenderer == null) return InteractionTypeEnum.NONE;
                if (itemsRenderer.inventoryManager.player.Controllable) return InteractionTypeEnum.INVENTORY_DRAG;
                return InteractionTypeEnum.NONE;
            }
        }

        public void Interact(IUser user)
        {
            isDragging = true;
            cameraDistance = itemsRenderer.inventoryManager.player.Camera.GlobalPosition.DistanceTo(GlobalPosition);
        }

        public override void _Process(double delta)
        {
            if (isDragging)
            {
                var camera = itemsRenderer.inventoryManager.player.Camera;
                var pos = itemsRenderer.inventoryManager.player.Camera.ProjectPosition(GetViewport().GetMousePosition(), cameraDistance);
                GlobalPosition = pos;
            }
        }

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