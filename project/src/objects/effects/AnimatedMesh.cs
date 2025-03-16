using Godot;
using Godot.Collections;

namespace Game
{
    public partial class AnimatedMesh : MeshInstance3D
    {
        [Export]
        public SpriteFrames Frames;
        [Export]
        public float SpeedScale = 1.0f;

        public int CurrentFrameIndex = 0;
        private float frameDelta = 0.0f;
        public string CurrentAnimationName = "default";

        public bool Playing = false;

        public override void _Ready()
        {

        }

        public void Play(string animationName = "default")
        {
            CurrentAnimationName = animationName;
            frameDelta = 0.0f;
            CurrentFrameIndex = 0;
            Playing = true;
        }

        public override void _Process(double delta)
        {
            if (!Playing) return;

            frameDelta += SpeedScale * (float)delta;
            if (frameDelta >= Frames.GetFrameDuration(CurrentAnimationName, CurrentFrameIndex) / Frames.GetAnimationSpeed(CurrentAnimationName))
            {
                frameDelta = 0.0f;
                CurrentFrameIndex += 1;
                if (CurrentFrameIndex >= Frames.GetFrameCount(CurrentAnimationName))
                {
                    CurrentFrameIndex = 0;
                }
            }
            var material = (ShaderMaterial)MaterialOverride;
            material.SetShaderParameter("textureAlbedo", Frames.GetFrameTexture(CurrentAnimationName, CurrentFrameIndex));
        }
    }
}