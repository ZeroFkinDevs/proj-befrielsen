using System;
using Godot;

namespace Game
{
    public interface IItemsStoring
    {
        public ItemsStorage ItemsStorageRes { get; }
    }
    public partial class ItemsStorage : Resource
    {
        [Export]
        public Godot.Collections.Array<ItemStack> ItemsStacks = new Godot.Collections.Array<ItemStack>();

        public event Action OnUpdate;


        public void AddItemStack(ItemStack stack)
        {
            ItemStack foundStack = null;
            foreach (var st in ItemsStacks)
            {
                if (st.ItemRes == stack.ItemRes)
                {
                    foundStack = st;
                    break;
                }
            }
            if (foundStack == null)
            {
                foundStack = new ItemStack();
                foundStack.ItemRes = stack.ItemRes;
                foundStack.Quantity = stack.Quantity;
                ItemsStacks.Add(foundStack);
            }
            else
            {
                foundStack.Quantity += stack.Quantity;
            }

            OnUpdate?.Invoke();
        }
        public void RemoveItemStack(ItemStack stack)
        {
            ItemsStacks.Remove(stack);
            OnUpdate?.Invoke();
        }
        public void AddItemStacks(Godot.Collections.Array<ItemStack> stacks)
        {
            foreach (var stack in stacks)
            {
                AddItemStack(stack);
            }
        }
        public void SetItemsStacks(Godot.Collections.Array<ItemStack> stacks)
        {
            ItemsStacks = stacks;
            OnUpdate?.Invoke();
        }
    }
}