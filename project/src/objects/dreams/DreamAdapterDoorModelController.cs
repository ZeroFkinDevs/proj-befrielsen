using Godot;
using Godot.Collections;

namespace Game
{
    public partial class DreamAdapterDoorModelController : AnimationController
    {

        public override void _Ready()
        {
            base._Ready();
        }

        public void Close()
        {
            SetState("state", "close");
        }
        public void Open()
        {
            SetState("state", "open");
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }
    }
}