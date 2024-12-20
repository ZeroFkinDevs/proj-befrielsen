using System;
using Godot;

namespace Game
{
	public partial class Player : CharacterBody3D, IUser, ILiving
	{
		public enum ControlGroupEnum
		{
			WORLD,
			HANDS,
			UI
		}

		[Export]
		public CharacterModel model;

		[Export]
		public InventoryManager inventoryManager;

		[Export]
		public bool Controllable = true;
		public ControlGroupEnum ControlGroup = ControlGroupEnum.WORLD;
		[Export]
		public Node3D Puppet;
		[Export]
		public float Gravity = 10.0f;
		[Export]
		public float Speed = 5.0f;
		[Export]
		public Camera3D Camera;
		[Export]
		public PlayerUI playerUI;
		[Export]
		public SmoothConnectTransform ModelSmoothConnector;
		[Export]
		public TmpStorage tmpStorage;
		[Export]
		public ObjectInstantiator objectInstantiator;
		[Export]
		public ObjectGrabber grabber;

		public Vector2 Movement;
		public Vector2 CameraRotation;
		public Vector2 CameraRotationTarget;
		private Vector3 LastGlobalPosition;

		[Export]
		public LivingStateManager livingStateManager { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _EnterTree()
		{
			var id = int.Parse(Name);
			SetMultiplayerAuthority(id);
			tmpStorage.SetMultiplayerAuthority(id);
			objectInstantiator.SpawnId += id;
		}
		public override void _Ready()
		{
			if (IsMultiplayerAuthority())
			{
				ModelSmoothConnector.NoSmooth = true;
				Controllable = true;
				if (Puppet != null)
				{
					Puppet.Visible = false;
				}
				model.DisableHead();
			}
			else
			{
				if (Puppet != null)
				{
					Puppet.Visible = true;
				}
				playerUI.QueueFree();
			}
			SetupCamera();
			LastGlobalPosition = GlobalPosition;
		}

		public void SetupCamera()
		{
			Camera.Current = Controllable;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			CommonProcess((float)delta);
			if (!Controllable) return;

			Movement.X += Input.GetAxis("move_left", "move_right");
			Movement.Y += Input.GetAxis("move_backward", "move_forward");
			Movement = Movement.Lerp(Vector2.Zero, 0.013f * 10.0f);
			if (Input.IsActionJustPressed("jump") && IsOnFloor())
			{
				Velocity = Velocity + Vector3.Up * 10.0f;
			}
			if (Input.IsActionJustPressed("exit"))
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}

			if (Input.IsActionJustPressed("inventory"))
			{
				ControlGroup = ControlGroupEnum.HANDS;
				inventoryManager.OpenInventory();
			}
			if (Input.IsActionJustReleased("inventory"))
			{
				ControlGroup = ControlGroupEnum.WORLD;
				inventoryManager.CloseInventory();
			}

			CameraRotation = CameraRotation.Lerp(CameraRotationTarget, (float)delta * 15.0f);
			var deg = GlobalRotation;
			deg.Y = CameraRotation.Y;
			GlobalRotation = deg;

			deg = Camera.GlobalRotation;
			deg.X = CameraRotation.X;
			Camera.GlobalRotation = deg;
		}

		void CommonProcess(float delta)
		{
			var boneTrans = model.GetBoneGlobalPose(model.NeckBoneID);
			Camera.GlobalPosition = Camera.GlobalPosition.Lerp(
				boneTrans.Origin - GlobalTransform.Basis.Z * 0.3f,
				0.2f);
		}

		public override void _Input(InputEvent @event)
		{
			if (!Controllable) return;
			if (ControlGroup == ControlGroupEnum.WORLD)
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
		}

		public override void _PhysicsProcess(double delta)
		{
			CommonProcess((float)delta);

			if (!Controllable) return;
			Vector3 newVelocity = Velocity;
			newVelocity.Y -= Gravity * (float)delta;

			Vector3 movementApplied = Vector3.Zero;
			movementApplied -= GlobalTransform.Basis.Z * Movement.Y;
			movementApplied += GlobalTransform.Basis.X * Movement.X;
			movementApplied.Y = 0;

			newVelocity += movementApplied * Speed;
			var stopForce = 0.2f;

			newVelocity.X = Mathf.Lerp(newVelocity.X, 0.0f, stopForce);
			newVelocity.Z = Mathf.Lerp(newVelocity.Z, 0.0f, stopForce);

			Velocity = newVelocity;
			MoveAndSlide();
			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				var collision = GetSlideCollision(i);
				ProcessCollision(collision, newVelocity);
			}
		}
		public void ProcessCollision(KinematicCollision3D collision, Vector3 velocity)
		{

		}

		[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
		public void RecievePosition(Vector3 pos)
		{
			GlobalPosition = pos;
		}
	}
}