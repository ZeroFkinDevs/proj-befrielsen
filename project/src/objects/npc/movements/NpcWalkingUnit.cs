using System;
using Godot;
using Godot.Collections;

namespace Game
{
	// этот класс предоставляет функции для управления body в физичном мире, но сам ни о чем не думает. решений не предпринимает
	public partial class NpcWalkingUnit : NpcMovementUnit
	{
		[Export]
		public float Gravity = 10.0f;
		[Export]
		public float Speed = 0.05f;

		public float StabilizeTimer = 0.5f;

		public Vector2 ControlMovement = Vector2.Zero;

		private Vector3 _smoothMotion = Vector3.Zero;
		public Vector3 MovementTarget = Vector3.Zero;
		public Vector3 LastGlobalPosition;

		public IWalkableModel WalkableModel
		{
			get
			{
				return npc.CharacterModel as IWalkableModel;
			}
		}

		public override void _Ready()
		{
			base._Ready();
			LastGlobalPosition = npc.GlobalPosition;
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			UpdateMotion((float)delta);
		}

		public void LookAtPoint(Vector3 point, float speed = 0.05f)
		{
			var trans = npc.GlobalTransform;
			point.Y = trans.Origin.Y;
			trans.Basis = new Basis(new Quaternion(trans.Basis).Normalized()).Slerp(new Basis(new Quaternion(trans.LookingAt(point).Basis).Normalized()), speed);
			npc.GlobalTransform = trans;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (!IsActive) return;
			UpdatePhysics((float)delta);
			ListenNpc((float)delta);
		}

		public void UpdatePhysics(float delta)
		{
			Vector3 newVelocity = npc.Velocity;
			newVelocity.Y -= Gravity * delta;

			Vector3 movementApplied = Vector3.Zero;
			movementApplied -= npc.GlobalTransform.Basis.Z * ControlMovement.Y;
			movementApplied += npc.GlobalTransform.Basis.X * ControlMovement.X;
			movementApplied.Y = 0;

			newVelocity += movementApplied * Speed;
			var stopForce = 0.05f;

			newVelocity.X = Mathf.Lerp(newVelocity.X, 0.0f, stopForce);
			newVelocity.Z = Mathf.Lerp(newVelocity.Z, 0.0f, stopForce);

			if (npc.IsOnFloor())
			{
				if (StabilizeTimer <= 0.0f)
				{
					var rot = npc.Rotation;
					if (rot.X > 0.0f || rot.X < 0.0f) rot.X /= 1.1f;
					if (rot.Z > 0.0f || rot.Z < 0.0f) rot.Z /= 1.1f;
					npc.Rotation = rot;
				}
			}
			StabilizeTimer -= delta;

			npc.Velocity = newVelocity;
			npc.MoveAndSlide();
		}

		public void UpdateMotion(float delta)
		{
			if (WalkableModel != null) WalkableModel.RecieveMotion(MovementTarget);
		}
		public void ListenNpc(float delta)
		{
			var modelMovement = (npc.GlobalPosition - LastGlobalPosition) / delta;
			modelMovement = npc.ToLocal(npc.GlobalPosition + modelMovement);
			if (npc.IsOnFloor())
			{
				modelMovement.Y = 0.0f;
			}
			else
			{
				modelMovement.Y = 100.0f;
			}

			MovementTarget = modelMovement;
			LastGlobalPosition = npc.GlobalPosition;
		}
	}
}