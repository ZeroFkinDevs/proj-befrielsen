using System.Threading.Tasks;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class SoundPlayer : AudioStreamPlayer3D
    {
        public override void _Ready()
        {
            base._Ready();
        }

        public Task PlaySound(string path)
        {
            Stream = ResourceLoader.Load<AudioStream>(path);
            Play();
            var eventAwait = new EventAwait()
            .OnConnect((func) => Finished += func)
            .OnDisconnect((func) => Finished -= func);
            return eventAwait.Listen();
        }
    }
}