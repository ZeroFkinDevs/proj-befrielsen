using Godot;
using Godot.Collections;

namespace Game
{
    public partial class Location : Node3D
    {
        [Export]
        public Array<NodePath> PlayerSpawners = new Array<NodePath>();
        [Export]
        public DreamsManager dreamsManager;
    }
}