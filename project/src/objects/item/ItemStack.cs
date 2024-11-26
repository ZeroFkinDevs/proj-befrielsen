using System;
using Godot;

namespace Game
{
    public partial class ItemStack : Resource
    {
        [Export]
        public ItemResource ItemRes;
        [Export]
        public int Quantity;
    }
}