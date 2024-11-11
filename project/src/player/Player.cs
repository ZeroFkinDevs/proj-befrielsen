using System;
using Godot;

namespace Game
{
	public partial class Player : CharacterBody3D
	{
		[Export]
		public float Gravity = 10.0f;
		[Export]
		public float Speed = 5.0f;
		[Export]
		public Camera3D Camera;

		public Vector2 Movement;
		public Vector2 CameraRotation;
		public Vector2 CameraRotationTarget;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			Movement.X += Input.GetAxis("move_left", "move_right");
			Movement.Y += Input.GetAxis("move_backward", "move_forward");
			Movement = Movement.Lerp(Vector2.Zero, (float)delta * 10.0f);
			if (Input.IsActionJustPressed("jump") && IsOnFloor())
			{
				Velocity = Velocity + Vector3.Up * 10.0f;
			}
			if (Input.IsActionJustPressed("exit"))
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}

			CameraRotation = CameraRotation.Lerp(CameraRotationTarget, (float)delta * 15.0f);
			var deg = GlobalRotation;
			deg.Y = CameraRotation.Y;
			GlobalRotation = deg;

			deg = Camera.GlobalRotation;
			deg.X = CameraRotation.X;
			Camera.GlobalRotation = deg;
		}

		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseMotion motion)
			{
				if (Input.MouseMode == Input.MouseModeEnum.Captured)
				{
					CameraRotationTarget.Y -= motion.Relative.X * 0.01f;
					CameraRotationTarget.X -= motion.Relative.Y * 0.01f;
					CameraRotationTarget.X = Mathf.Clamp(CameraRotationTarget.X, -Mathf.Pi / 2.0f, Mathf.Pi / 2.0f);
				}
			}
			if (@event is InputEventMouseButton button)
			{
				if (button.ButtonIndex == MouseButton.Left)
				{
					Input.MouseMode = Input.MouseModeEnum.Captured;
				}
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector3 newVelocity = Velocity;
			newVelocity.Y -= Gravity * (float)delta;

			// newVelocity.X = 0;
			// newVelocity.Z = 0;

			Vector3 movementApplied = Vector3.Zero;
			movementApplied -= GlobalTransform.Basis.Z * Movement.Y;
			movementApplied += GlobalTransform.Basis.X * Movement.X;
			movementApplied.Y = 0;

			newVelocity += movementApplied * Speed;
			var stopForce = 4.0f;
			if (IsOnFloor())
			{
				stopForce = 10.0f;
			}
			newVelocity.X = Mathf.Lerp(newVelocity.X, 0.0f, (float)delta * stopForce);
			newVelocity.Z = Mathf.Lerp(newVelocity.Z, 0.0f, (float)delta * stopForce);

			Velocity = newVelocity;
			MoveAndSlide();
		}
	}
}