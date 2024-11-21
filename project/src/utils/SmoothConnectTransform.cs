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
		public override void _Process(double delta)
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
				if (!trans.Basis.IsEqualApprox(Target.GlobalTransform.Basis))
				{
					try
					{
						trans.Basis = trans.Basis.Slerp(Target.GlobalTransform.Basis, (float)delta * Speed);
					}
					catch (Exception e)
					{

					}
				}
			}
			Object.GlobalTransform = trans;
		}
	}
}