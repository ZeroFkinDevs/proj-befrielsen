using Godot;
using Godot.Collections;

namespace Game
{
    public partial class Location : Node3D
    {
        [Export]
        public Array<Node3D> PlayerSpawners = new Array<Node3D>();
    }
}