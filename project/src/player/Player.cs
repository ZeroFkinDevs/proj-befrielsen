using System;
using Godot;
using Game.Utils;
using System.Collections;

namespace Game
{
	public partial class Player : CharacterBody3D, IUser, ILiving, IProvidingTeleportPoint
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
		public bool IsPuppet;
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

		public float StabilizeTimer = 0.5f;

		public Action OnTeleported;

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
				model.DisableHead();
			}
			else
			{
				playerUI.Visible = false;
				Controllable = false;
			}
			if (Multiplayer.IsServer())
			{
				playerUI.Visible = false;
			}
			SetupCamera();
			LastGlobalPosition = GlobalPosition;
		}

		public void SetupCamera()
		{
			Camera.Current = Controllable && !IsPuppet;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			CommonProcess((float)delta);
			if (!Controllable) return;

			// setting 11 layer to all meshes inside
			if (!IsPuppet)
			{
				foreach (var child in this.GetChildrenRecursively())
				{
					if (child is MeshInstance3D mesh)
					{
						if (!mesh.GetLayerMaskValue(11)) mesh.SetLayerMaskValue(11, true);
					}
				}
			}

			// controls
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

			CameraRotation = CameraRotation.Lerp(Vector2.Zero, (float)delta * 15f);
			var deg = GlobalRotation;
			deg.Y += CameraRotation.Y * (float)delta;
			GlobalRotation = deg;

			deg = Camera.GlobalRotation;
			deg.X += CameraRotation.X * (float)delta;
			deg.X = Mathf.Clamp(deg.X, -Mathf.Pi / 2.0f, Mathf.Pi / 2.0f);
			Camera.GlobalRotation = deg;

			deg = Camera.Rotation;
			if (deg.Z > 0.0f || deg.Z < 0.0f) deg.Z /= 1.1f;
			if (deg.Y > 0.0f || deg.Y < 0.0f) deg.Y /= 1.1f;
			Camera.Rotation = deg;
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
						CameraRotation.Y -= motion.Relative.X * 0.15f;
						CameraRotation.X -= motion.Relative.Y * 0.15f;
						// CameraRotation.X = Mathf.Clamp(CameraRotation.X, -Mathf.Pi / 2.0f, Mathf.Pi / 2.0f);
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
			if (IsPuppet) return;

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

			if (IsOnFloor())
			{
				if (StabilizeTimer <= 0.0f)
				{
					var rot = Rotation;
					if (rot.X > 0.0f || rot.X < 0.0f) rot.X /= 1.1f;
					if (rot.Z > 0.0f || rot.Z < 0.0f) rot.Z /= 1.1f;
					Rotation = rot;
				}
			}
			StabilizeTimer -= (float)delta;

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

		public void Teleport(Transform3D transform)
		{
			Rpc(MethodName.RecieveTeleport, transform);
		}
		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
		public void RecieveTeleport(Transform3D transform)
		{
			GlobalTransform = transform;
			ModelSmoothConnector.Teleport();
			OnTeleported?.Invoke();
		}

		public void Despawn()
		{
			grabber.RequestUngrabProp(0.0f);
			RpcId(1, MethodName.QueueFreeBroadcast);
		}

		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
		public void QueueFreeBroadcast()
		{
			Rpc(MethodName.QueueFreeRecieve);
		}
		[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
		public void QueueFreeRecieve()
		{
			QueueFree();
		}

		public Vector3 GetTeleportPoint()
		{
			return Camera.GlobalPosition;
		}
	}
}