using System;
using Godot;

namespace Game
{
    public partial class ItemResource : Resource
    {
        [Export]
        public PackedScene MeshScene;
        [Export]
        public PackedScene PropScene;
        [Export]
        public string Name;
    }
}