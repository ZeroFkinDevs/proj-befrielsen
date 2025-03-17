using Godot;
using Godot.Collections;

namespace Game
{
	public partial class PropPusher : Area3D
	{
		[Export]
		public Player player;

		private float _timer = 0.0f;

		public override void _PhysicsProcess(double delta)
		{
			if (player.Current) PushCycle(delta);
		}

		public void PushCycle(double delta)
		{
			if (_timer > 0.0f)
			{
				_timer -= (float)delta;
			}
			if (_timer <= 0.0f)
			{
				CheckPush();
				_timer = 0.2f;
			}
		}

		public void CheckPush()
		{
			var bodies = GetOverlappingBodies();
			foreach (var body in bodies)
			{
				if (body is Prop prop)
				{
					prop.RequestImpulse(player.Velocity * 1.0f * new Vector3(1, 0, 1), null);
				}
			}
		}
	}
}