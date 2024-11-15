using Godot;
using Godot.Collections;

namespace Game
{
	public partial class SmoothConnectTransform : Node3D
	{
		[Export]
		public Node3D Target;

		public override void _Process(double delta)
		{
			var trans = GlobalTransform;
			trans.Origin = trans.Origin.Slerp(Target.GlobalTransform.Origin, (float)delta * 16.0f);
			trans.Basis = trans.Basis.Slerp(Target.GlobalTransform.Basis, (float)delta * 16.0f);
			GlobalTransform = trans;
		}
	}
}