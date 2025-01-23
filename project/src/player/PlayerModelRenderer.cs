using Godot;
using System;

namespace Game
{
    public partial class PlayerModelRenderer : Node3D
    {
        [Export]
        public Player player;
        [Export]
        public Camera3D layerCamera;
        [Export]
        public SubViewport layerViewport;

        public override void _Ready()
        {
        }
        public override void _Process(double delta)
        {
            var curViewport = GetViewport();
            layerViewport.Size = new Vector2I((int)curViewport.GetVisibleRect().Size.X / 1, (int)curViewport.GetVisibleRect().Size.Y / 1);
            layerViewport.Msaa3D = curViewport.Msaa3D;
            layerViewport.ScreenSpaceAA = curViewport.ScreenSpaceAA;
            layerViewport.UseTaa = curViewport.UseTaa;
            layerViewport.UseDebanding = curViewport.UseDebanding;
            layerViewport.UseOcclusionCulling = curViewport.UseOcclusionCulling;
            layerViewport.MeshLodThreshold = curViewport.MeshLodThreshold;

            if (player.IsPuppet)
            {
                layerCamera.Current = false;
            }

            layerCamera.GlobalTransform = player.Camera.GlobalTransform;
        }
    }
}