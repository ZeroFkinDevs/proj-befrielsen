using System;
using System.ComponentModel;
using System.Net.Http;
using Godot;
using Game.Utils;

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
		public Vector3 LastGlobalPosition;

		public delegate bool ObserveLockCameraStateDelegate();
		public ObserveLockCameraStateDelegate ObserveLockToCamera;
		public bool LockToCamera
		{
			get
			{
				foreach (var func in ObserveLockToCamera.GetInvocationList())
				{
					if ((bool)func.DynamicInvoke())
					{
						return true;
					}
				}
				return false;
			}
		}

		public int NeckBoneID = 0;
		public int HeadBoneID = 0;
		public int LeftHandBoneID = 0;
		public int RightHandBoneID = 0;
		public int RightToolBoneID = 0;
		public int ArmsRotatorBoneID = 0;
		public float ArmsLockFactor = 0.0f;

		public static class HandsActionType
		{
			public static string NONE = null;
			public static string STICK_ATTACK = "stick_attack";
		}

		public string handsActionType = HandsActionType.NONE;

		public override void _EnterTree()
		{
			ModelScale = 0.155f;
		}

		public override void _Ready()
		{
			base._Ready();
			NeckBoneID = skeleton3D.FindBone("neck");
			HeadBoneID = skeleton3D.FindBone("head");
			LeftHandBoneID = skeleton3D.FindBone("arm_l_3");
			RightHandBoneID = skeleton3D.FindBone("arm_r_3");
			RightToolBoneID = skeleton3D.FindBone("arm_r_tool");
			ArmsRotatorBoneID = skeleton3D.FindBone("arms_rotator");
			SetHandsAction(handsActionType);
		}

		public void SetHandsAction(string action = null)
		{
			handsActionType = action;
			if (action != null)
			{
				SetState("action_type", action);
			}
		}
		public void TriggerOneShotAction()
		{
			if (handsActionType != null)
			{
				OneShot("one_shot_action_" + handsActionType);
			}
		}

		public void SetHandsContinousState(string state)
		{
			SetState("hands_continous_state", state);
		}

		public void DisableHead()
		{
			skeleton3D.SetBonePoseScale(NeckBoneID, Vector3.One * 0.02f);
		}

		public void UpdateAction(float delta)
		{
			if (handsActionType == null)
			{
				SetBlend("blend_hands_action", Mathf.Lerp(GetBlend("blend_hands_action"), 0.0f, delta * 20.0f));
			}
			else
			{
				SetBlend("blend_hands_action", Mathf.Lerp(GetBlend("blend_hands_action"), 1.0f, delta * 20.0f));
			}
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
			UpdateAction((float)delta);
		}
		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);
			ListenPlayer((float)delta);
		}
	}
}