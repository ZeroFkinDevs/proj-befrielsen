using Godot;
using Godot.Collections;

namespace Game
{
	public interface IWalkableModel
	{
		public void RecieveMotion(Vector3 motion);
		public void StartJumping();
		public void Ground();
		public Vector3 GetMovementVector();
	}

	public partial class NpcWalkableModel : NpcCharacterModel, IWalkableModel
	{
		public float MovementSensivity = 1.0f;
		Vector3 _motion;
		Vector3 _smoothMotion;

		int movementBoneId;

		public override void _Ready()
		{
			base._Ready();
			movementBoneId = skeleton3D.FindBone("movement");
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			UpdateMotion((float)delta);
		}

		public void UpdateMotion(float delta)
		{
			_smoothMotion = _smoothMotion.Lerp(_motion, (float)delta * 20.0f);
			var WalkSpeed = (_smoothMotion * new Vector3(1.0f, 0.0f, 1.0f)).Length();

			if (_smoothMotion.Y > 50.0f)
			{
				// SetState("legs_state", "air");
				// SetState("hands_state", "walk");
				// SetBlend2D("hands_movement_blend", GetBlend2D("hands_movement_blend").Lerp(
				// 		new Vector2(1.0f, 0.0f).Normalized(), delta * 20.0f));
			}
			else
			{
				// if (GetState("legs_state") == "air")
				// {
				// 	SetState("grounded_state", "grounded");
				// }

				SetState("legs_state", "walk");
				SetState("hands_state", "walk");

				SetTimeScale("walk_speed", WalkSpeed * MovementSensivity);
				SetTimeScale("hands_speed", WalkSpeed * MovementSensivity);

				SetBlend2D("movement_blend", GetBlend2D("movement_blend").Lerp(
					new Vector2(_smoothMotion.X, _smoothMotion.Z), delta * 20.0f));
				// SetBlend2D("hands_movement_blend", GetBlend2D("hands_movement_blend").Lerp(
				// 	new Vector2(_smoothMotion.X, _smoothMotion.Z).Normalized(), delta * 20.0f));
			}
		}

		public virtual void RecieveMotion(Vector3 motion)
		{
			_motion = motion;
		}

		public void StartJumping()
		{
			throw new System.NotImplementedException();
		}

		public void Ground()
		{
			throw new System.NotImplementedException();
		}

		public Vector3 GetMovementVector()
		{
			return ToLocal(GetBoneGlobalPose(movementBoneId).Origin);
		}
	}
}