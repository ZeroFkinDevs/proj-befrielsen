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
            ItemStack foundStack = FindStack(stack.ItemRes);
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
        public ItemStack FindStack(ItemResource itemResource)
        {
            ItemStack foundStack = null;
            foreach (var st in ItemsStacks)
            {
                if (st.ItemRes == itemResource)
                {
                    foundStack = st;
                    break;
                }
            }
            return foundStack;
        }
        public bool HasItem(ItemResource itemResource, int amount = 1)
        {
            ItemStack foundStack = FindStack(itemResource);
            if (foundStack != null)
            {
                if (foundStack.Quantity < amount) return false;
                return true;
            }
            return false;
        }
        public bool ConsumeItem(ItemResource itemResource, int amount)
        {
            if (!HasItem(itemResource, amount)) return false;

            ItemStack foundStack = FindStack(itemResource);
            foundStack.Quantity -= amount;
            if (foundStack.Quantity == 0)
            {
                RemoveItemStack(foundStack);
                return true;
            }
            OnUpdate?.Invoke();
            return true;
        }
        public void SetItemsStacks(Godot.Collections.Array<ItemStack> stacks)
        {
            ItemsStacks = stacks;
            OnUpdate?.Invoke();
        }
    }
}