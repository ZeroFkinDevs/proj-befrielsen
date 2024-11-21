using System;
using System.Net.Http;
using Godot;

namespace Game
{
	public partial class CharacterModel : AnimationController
	{
		[Export]
		public float MovementSensivity = 1.0f;

		private Vector3 _movement;
		private Vector3 _smoothMovement;
		[Export]
		public Vector3 MovementTarget;

		public int NeckBoneID = 0;
		public void UpdateMovement(float delta)
		{
			_smoothMovement = _smoothMovement.Lerp(MovementTarget, (float)delta * 20.0f);
			// Vector3 direction = (_smoothMovement * new Vector3(1.0f, 0.0f, 1.0f)).Normalized();
			var WalkSpeed = (_smoothMovement * new Vector3(1.0f, 0.0f, 1.0f)).Length();

			if (WalkSpeed < 1.0f)
			{
				SetState("legs_state", "idle");
			}
			else
			{
				SetState("legs_state", "walk");

				SetTimeScale("walk_speed", WalkSpeed * MovementSensivity);

				SetBlend2D("movement_blend", new Vector2(_smoothMovement.X, _smoothMovement.Z).Normalized());
			}
		}

		public override void _Ready()
		{
			base._Ready();
			NeckBoneID = skeleton3D.FindBone("neck");
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			UpdateMovement((float)delta);
		}
	}
}