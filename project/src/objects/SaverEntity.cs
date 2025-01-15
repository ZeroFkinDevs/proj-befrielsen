using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
	public partial class SaverEntity : Node3D, IInteractable
	{
		public LocationLoader locationLoader { get { return this.GetMultiplayerNode<LocationLoader>("/root/Main/Client/LocationLoader"); } }
		public float reloadTime = 0.0f;
		public bool reloading { get { return reloadTime > 0.0f; } }
		public InteractionTypeEnum InteractionType => !reloading ? InteractionTypeEnum.TOUCH : InteractionTypeEnum.NONE;

		public string InteractableDescription => "save the world";

		public void Interact(IUser user)
		{
			reloadTime = 1.0f;
			locationLoader.RequestSaveLocation();
		}
		public override void _Process(double delta)
		{
			if (reloadTime > 0.0f)
			{
				reloadTime -= (float)delta;
			}
		}
	}
}