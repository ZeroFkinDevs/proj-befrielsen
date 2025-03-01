using System;
using System.ComponentModel;
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
		public Transform3D trans;

		public override void _Ready()
		{
			trans = Object.GlobalTransform;
		}
		public override void _PhysicsProcess(double delta)
		{
			if (NoSmooth)
			{
				trans = Target.GlobalTransform;
			}
			else
			{
				if (!trans.Origin.IsEqualApprox(Target.GlobalTransform.Origin))
				{
					trans.Origin = trans.Origin.Slerp(Target.GlobalTransform.Origin, (float)delta * Speed);
				}
				try
				{
					trans.Basis = trans.Basis.Slerp(Target.GlobalTransform.Basis, (float)delta * Speed);
				}
				catch (Exception e)
				{
					trans = Target.GlobalTransform;
					// GD.PrintErr(e);
				}
			}
			Object.GlobalTransform = trans;
		}

		public void Teleport()
		{
			trans = Target.GlobalTransform;
			Object.GlobalTransform = trans;
			if (Object is ModelWithFreeBone modelWithFreeBone)
			{
				modelWithFreeBone.ResetPose();
			}
		}
	}
}