using Godot;
using Godot.Collections;

namespace Game
{
    public partial class EyeToolItemResource : ToolItemResource
    {
        [Export]
        public Array<PackedScene> dreams;
        [Export]
        public string DreamCode = "";
    }
}