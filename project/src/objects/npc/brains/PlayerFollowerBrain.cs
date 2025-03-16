using Godot;
using Godot.Collections;

namespace Game
{
	public partial class PlayerFollowerBrain : NpcBrain
	{
		[Export]
		public Area3D playerCheckArea;
		Player player;

		public NpcWalkingController walkingUnit
		{
			get
			{
				if (!IsActive) return null;
				return npc.MovementUnit as NpcWalkingController;
			}
		}

		public override void _Ready()
		{
			playerCheckArea.BodyEntered += OnBodyEntered;
			playerCheckArea.BodyExited += OnBodyExited;
		}

		public void OnBodyEntered(Node3D body)
		{
			if (body is Player _pl) player = _pl;
		}
		public void OnBodyExited(Node3D body)
		{
			if (body is Player _pl) player = null;
		}

		public override void _Process(double delta)
		{
			if (walkingUnit == null) return;
			walkingUnit.ControlMovement = Vector3.Zero;

			if (!IsActive) return;
			if (player == null) return;

			var distance = player.GlobalPosition.DistanceTo(npc.GlobalPosition);
			if (distance > 2.5f)
			{
				walkingUnit.ControlMovement = Vector3.Forward;
				walkingUnit.LookAtPoint(player.GlobalPosition);
			}
			else if (distance <= 1.4f)
			{
				walkingUnit.ControlMovement = Vector3.Back * 2;
				walkingUnit.LookAtPoint(player.GlobalPosition);
			}
		}
	}
}