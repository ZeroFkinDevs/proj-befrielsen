using System;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class ItemResource : Resource
    {
        [Export]
        public PackedScene MeshScene;
        [Export]
        public string PropScenePath;
        [Export]
        public string Name;

        public (Node3D, Prop) InstantiateSimpleProp(Node3D parent)
        {
            var packedScene = GD.Load<PackedScene>(PropScenePath);
            var node = packedScene.Instantiate<Node3D>();
            Prop prop = null;
            foreach (var child in node.GetChildren())
            {
                if (child is Prop foundProp)
                {
                    prop = foundProp;
                }
            }
            prop.itemsStorage = new ItemsStorage();
            var stack = new ItemStack();
            stack.Quantity = 1;
            stack.ItemRes = this;
            prop.itemsStorage.AddItemStack(stack);
            parent.AddChild(node);
            return (node, prop);
        }
    }
}