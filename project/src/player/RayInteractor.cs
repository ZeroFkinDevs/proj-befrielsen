using Godot;
using Godot.Collections;

namespace Game
{
	public partial class RayInteractor : Node3D
	{
		[Export]
		public RayCast3D UseRaycast;
		[Export]
		public Player player;
		[Export]
		public ObjectGrabber grabber;
		public bool Enabled = true;
		public IInteractable Interactable = null;

		public override void _Process(double delta)
		{
			if (!player.Controllable) return;

			Interactable = null;

			if (!Enabled) return;
			if (grabber.IsGrabbing) return;

			if (UseRaycast.IsColliding())
			{
				if (UseRaycast.GetCollider() is IInteractable usable)
				{
					Interactable = usable;
				}
			}

			if (Interactable != null && Input.IsActionJustPressed("use"))
			{
				if (Interactable.InteractionType == InteractionTypeEnum.GRAB)
				{
					grabber.Grab(Interactable);
				}
			}
		}
	}
}