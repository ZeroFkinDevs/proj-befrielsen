using System;
using System.Net.Http;
using Godot;

namespace Game
{
	public partial class CharacterModel : AnimationController
	{
		[Export]
		public float MovementSensivity = 1.0f;

		[Export]
		public Player player;

		private Vector3 _movement;
		private Vector3 _smoothMovement;
		[Export]
		public Vector3 MovementTarget;
		[Export]
		public Vector3 HandsState;
		private Vector3 LastGlobalPosition;

		public bool LockToCamera = false;

		public int NeckBoneID = 0;
		public int HeadBoneID = 0;
		public int ArmsRotatorBoneID = 0;
		public float ArmsLockFactor = 0.0f;

		public override void _Ready()
		{
			base._Ready();
			NeckBoneID = skeleton3D.FindBone("neck");
			HeadBoneID = skeleton3D.FindBone("head");
			ArmsRotatorBoneID = skeleton3D.FindBone("arms_rotator");
		}

		public void SetHandsContinousState(string state)
		{
			SetState("hands_continous_state", state);
		}

		public void UpdateMovement(float delta)
		{
			_smoothMovement = _smoothMovement.Lerp(MovementTarget, (float)delta * 20.0f);
			// Vector3 direction = (_smoothMovement * new Vector3(1.0f, 0.0f, 1.0f)).Normalized();
			var WalkSpeed = (_smoothMovement * new Vector3(1.0f, 0.0f, 1.0f)).Length();

			if (MovementTarget.Y > 50.0f)
			{
				SetState("legs_state", "air");
				SetState("hands_state", "walk");
				SetBlend2D("hands_movement_blend", GetBlend2D("hands_movement_blend").Lerp(
						new Vector2(1.0f, 0.0f).Normalized(), delta * 20.0f));
			}
			else
			{
				if (GetState("legs_state") == "air")
				{
					SetState("grounded_state", "grounded");
				}

				if (WalkSpeed < 1.0f)
				{
					SetState("legs_state", "idle");
					SetState("hands_state", "idle");
				}
				else
				{
					SetState("legs_state", "walk");
					SetState("hands_state", "walk");

					SetTimeScale("walk_speed", WalkSpeed * MovementSensivity);
					SetTimeScale("hands_speed", WalkSpeed * MovementSensivity);

					SetBlend2D("movement_blend", GetBlend2D("movement_blend").Lerp(
						new Vector2(_smoothMovement.X, _smoothMovement.Z).Normalized(), delta * 20.0f));
					SetBlend2D("hands_movement_blend", GetBlend2D("hands_movement_blend").Lerp(
						new Vector2(_smoothMovement.X, _smoothMovement.Z).Normalized(), delta * 20.0f));
				}
			}
		}

		public void ListenPlayer(float delta)
		{
			var trans = skeleton3D.GlobalTransform.AffineInverse() * (player.Camera.GlobalTransform.TranslatedLocal(new Vector3(0, 0, 0.1f)));
			trans = trans
					.ScaledLocal(new Vector3(0.155f, 0.155f, 0.155f))
					.RotatedLocal(Vector3.Up, Mathf.Pi);
			if (player.Controllable && LockToCamera)
			{
				ArmsLockFactor = Mathf.Lerp(ArmsLockFactor, 1.0f, delta * 3.0f);
			}
			else
			{
				ArmsLockFactor = Mathf.Lerp(ArmsLockFactor, 0.0f, delta);
			}
			skeleton3D.SetBoneGlobalPoseOverride(ArmsRotatorBoneID, trans, ArmsLockFactor, true);

			skeleton3D.SetBonePoseRotation(HeadBoneID, Quaternion.FromEuler(new Vector3(-player.Camera.Rotation.X / 2.0f, 0, 0)));
			skeleton3D.SetBonePoseRotation(NeckBoneID, Quaternion.FromEuler(new Vector3(-player.Camera.Rotation.X / 3.0f, 0, 0)));
			if (LockToCamera)
			{
				SetState("hands_lock_state", "locked");
				skeleton3D.SetBonePoseRotation(ArmsRotatorBoneID, Quaternion.FromEuler(new Vector3(-player.Camera.Rotation.X, 0, 0)));
			}
			else
			{
				SetState("hands_lock_state", "rest");
				skeleton3D.SetBonePoseRotation(ArmsRotatorBoneID, Quaternion.FromEuler(new Vector3(0, 0, 0)));
			}

			if (player.Controllable)
			{
				var modelMovement = (player.GlobalPosition - LastGlobalPosition) / delta;
				modelMovement = player.ToLocal(player.GlobalPosition + modelMovement);
				if (player.IsOnFloor())
				{
					modelMovement.Y = 0.0f;
				}
				else
				{
					modelMovement.Y = 100.0f;
				}

				MovementTarget = modelMovement;
				LastGlobalPosition = player.GlobalPosition;
			}
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			UpdateMovement((float)delta);
		}
		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);
			ListenPlayer((float)delta);
		}
	}
}