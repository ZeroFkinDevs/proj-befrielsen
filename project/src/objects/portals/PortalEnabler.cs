using Godot;
using Godot.Collections;
using Game.Utils;

namespace Game
{
    public partial class PortalEnabler : Area3D
    {
        [Export]
        public Camera3D camera;

        public override void _Ready()
        {
            AreaEntered += OnBodyEntered;
            AreaExited += OnBodyExited;
        }

        public void OnBodyEntered(Node3D body)
        {
            if (!camera.Current) return;
            if (body is Portal portal)
            {
                portal.Enable();
            }
        }
        public void OnBodyExited(Node3D body)
        {
            if (!camera.Current) return;
            if (body is Portal portal)
            {
                portal.Disable();
            }
        }
    }
}