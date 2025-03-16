using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Game.Dialog
{
    public interface ISubtitlesProvider
    {
        public SubtitlesUnit subtitlesUnit { get; }
    }

    public partial class SubtitlesUnit : Node3D
    {
        [Export]
        public Label3D label;

        public event Func<Node3D> TargetSubscription;
        public string CurrentText
        {
            get => label.Text; set
            {
                label.Text = value;
            }
        }
        public event Action OnChanged;

        public async void PlaySubtitlesForTask(string text, Task task)
        {
            CurrentText = text;
            OnChanged?.Invoke();
            await task;
            CurrentText = "";
            OnChanged?.Invoke();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            Node3D target = null;
            if (TargetSubscription != null)
            {
                foreach (var func in TargetSubscription.GetInvocationList())
                {
                    target = (Node3D)func.DynamicInvoke();
                }
            }
            if (target != null)
            {
                label.Visible = true;
                LookAt(target.GlobalTransform.Origin);
            }
            else
            {
                label.Visible = false;
            }
        }
    }
}