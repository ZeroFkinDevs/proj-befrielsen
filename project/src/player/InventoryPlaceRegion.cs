using System;
using Godot;

namespace Game
{
    public interface IInventoryPlaceRegion
    {
        public void OnEnter(InventoryItemStackRenderer stackRenderer);
        public void OnExit();
        public void Interact(InventoryItemStackRenderer stackRenderer);
    }
    public partial class InventoryPlaceRegion : Area3D, IInventoryPlaceRegion
    {
        [Export]
        public InventoryContainer inventoryContainer;

        public override void _EnterTree()
        {
            OnExit();
        }

        public void OnEnter(InventoryItemStackRenderer stackRenderer)
        {
            Visible = true;
        }

        public void OnExit()
        {
            Visible = false;
        }

        public void Interact(InventoryItemStackRenderer stackRenderer)
        {
            stackRenderer.itemsRenderer.inventoryContainer.AddItemStacks(new Godot.Collections.Array<ItemStack> { stackRenderer.itemStack });
            stackRenderer.QueueFree();
        }
    }
}