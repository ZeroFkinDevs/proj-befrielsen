using Godot;
using Godot.Collections;

namespace Game
{
	public partial class SmoothConnectTransform : Node3D
	{
		[Export]
		public Node3D Object;
		[Export]
		public Node3D Target;
		[Export]
		public float Speed = 16.0f;
		[Export]
		public bool NoSmooth = false;

		public override void _Process(double delta)
		{
			var trans = Object.GlobalTransform;
			if (NoSmooth)
			{
				trans = Target.GlobalTransform;
			}
			else
			{
				trans.Origin = trans.Origin.Slerp(Target.GlobalTransform.Origin, (float)delta * Speed);
				trans.Basis = trans.Basis.Slerp(Target.GlobalTransform.Basis, (float)delta * Speed);
			}
			Object.GlobalTransform = trans;
		}
	}
}