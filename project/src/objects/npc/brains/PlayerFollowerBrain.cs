using Godot;
using Godot.Collections;

namespace Game
{
	public partial class PlayerFollowerBrain : NpcBrain
	{
		[Export]
		public Area3D playerCheckArea;
		Player player;

		public NpcWalkingUnit walkingUnit
		{
			get
			{
				if (!IsActive) return null;
				return npc.GetMovementUnit<NpcWalkingUnit>();
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
			walkingUnit.ControlMovement = Vector2.Zero;

			if (!IsActive) return;
			if (player == null) return;

			walkingUnit.LookAtPoint(player.GlobalPosition);

			if (player.GlobalPosition.DistanceTo(npc.GlobalPosition) > 2.0f)
			{
				walkingUnit.ControlMovement = Vector2.Down;
			}

		}
	}
}