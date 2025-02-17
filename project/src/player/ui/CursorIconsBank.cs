using System;
using Godot;

namespace Game
{
    public partial class CursorIconsBank : Resource
    {
        [Export]
        public Texture2D Default;
        [Export]
        public Texture2D Grab;
        [Export]
        public Texture2D PICKUP;
        [Export]
        public Texture2D InventoryDrag;
        [Export]
        public Texture2D Apply;
        [Export]
        public Texture2D Touch;
    }
}