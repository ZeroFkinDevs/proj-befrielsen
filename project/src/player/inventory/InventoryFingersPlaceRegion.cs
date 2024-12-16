using System;
using Godot;

namespace Game
{
    public partial class InventoryFingersPlaceRegion : Area3D, IInventoryPlaceRegion
    {
        [Export]
        public InventoryContainer inventoryContainer;

        public bool ItemFits(ItemResource itemRes)
        {
            return itemRes is FingerItemResource;
        }

        public void Interact(InventoryItemStackRenderer stackRenderer)
        {
            if (!ItemFits(stackRenderer.itemStack.ItemRes)) return;
            var res = inventoryContainer.inventoryManager.player.livingStateManager.Heal(1);
            if (res)
            {
                stackRenderer.ConsumeOne();
            }
        }

        public void OnEnter(InventoryItemStackRenderer stackRenderer)
        {
            Visible = ItemFits(stackRenderer.itemStack.ItemRes);
        }

        public void OnExit()
        {
            Visible = false;
        }
    }
}