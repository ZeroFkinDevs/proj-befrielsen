using Godot;
using Godot.Collections;

namespace Game
{
    public partial class LayeredEffect : Node3D
    {
        [Export]
        public string frameNameBase;
        [Export]
        public float AnimationSpeed = 4.0f;
        [Export]
        public int FramesCount = 10;
        [Export]
        public int LayersCount = 5;
        [Export]
        public float LayersDistance = 0.5f;
        [Export]
        public string framesDirectoryPath;
        [Export]
        public float CommonScale;

        public override void _Ready()
        {
            GeneratePlanes();
        }
        public void GeneratePlanes()
        {
            for (int i = 0; i < LayersCount; i++)
            {
                GenerateLayerPlane(i);
            }
        }
        public SpriteFrames LoadSpriteFrames(int layer)
        {
            var frames = new SpriteFrames();

            for (int i = 0; i < FramesCount; i++)
            {
                var imageName = frameNameBase + "_layer_" + layer + "_" + i.ToString("0000") + ".png";
                var framePath = framesDirectoryPath + imageName;
                var frameTexture = ResourceLoader.Load<Texture2D>(framePath);
                frames.AddFrame("default", frameTexture);
            }
            return frames;
        }
        public void GenerateLayerPlane(int layer)
        {
            var plane = new AnimatedSprite3D();
            AddChild(plane);
            var pos = (-((float)LayersCount / 2.0f) + (float)layer) * LayersDistance * -1;
            plane.Position = new Vector3(0.0f, 0.0f, pos);
            plane.Scale = Vector3.One * CommonScale;
            plane.SpriteFrames = LoadSpriteFrames(layer);

            plane.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
            plane.Play("default", AnimationSpeed);
        }
        public override void _Process(double delta)
        {
            var camera = GetViewport().GetCamera3D();
            if (camera == null) return;

            var trans = GlobalTransform;
            trans.Basis = new Basis(new Quaternion(trans.Basis).Normalized()).Slerp(new Basis(new Quaternion(trans.LookingAt(camera.GlobalPosition).Basis).Normalized()), 0.05f);
            GlobalTransform = trans;
        }
    }
}