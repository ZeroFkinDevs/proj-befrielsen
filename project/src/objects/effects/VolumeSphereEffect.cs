using Godot;
using Godot.Collections;

namespace Game
{
    public partial class VolumeSphereEffect : Node3D
    {
        [Export]
        public string frameNameBase;
        [Export]
        public float AnimationSpeed = 4.0f;
        [Export]
        public int FramesCount = 20;
        [Export]
        public int LayersCount = 5;
        [Export]
        public string framesDirectoryPath;
        [Export]
        public float CommonScale;

        [Export]
        public Mesh LayerMesh;
        [Export]
        public Shader shader;

        public override void _Ready()
        {
            GeneratePlanes();
        }
        public void GeneratePlanes()
        {
            for (int i = 0; i < LayersCount; i++)
            {
                GenerateLayerSphere(i);
            }
        }
        public SpriteFrames LoadLayerTexture(int layer)
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
        public void GenerateLayerSphere(int layer)
        {
            var layerSphere = new AnimatedMesh();
            AddChild(layerSphere);

            layerSphere.Mesh = LayerMesh;
            var material = new ShaderMaterial();
            material.Shader = shader;
            layerSphere.MaterialOverride = material;
            layerSphere.Frames = LoadLayerTexture(layer);
            layerSphere.SpeedScale = AnimationSpeed;
            material.SetShaderParameter("scale", (1 + (layer * 1.0f)));
            layerSphere.Play("default");
        }
        public override void _Process(double delta)
        {
            var rot = Rotation;
            rot.Y += (float)delta;
            Rotation = rot;
        }
    }
}