using System;
using Godot;

namespace Game
{
	public partial class Cursor : Control
	{
		[Export]
		public PlayerUI playerUI;
		[Export]
		public CursorIconsBank cursorIconsBank;
		[Export]
		public RayInteractor rayInteractor;

		[Export]
		public TextureRect textureRect;

		[Export]
		public Label descriptionLabel;

		public override void _Process(double delta)
		{
			GlobalPosition = GetGlobalMousePosition();
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				GlobalPosition = GetViewportRect().Size / 2.0f;
			}

			rayInteractor.UseRaycast.TargetPosition = rayInteractor.UseRaycast.ToLocal(rayInteractor.player.Camera.ProjectPosition(GlobalPosition, 2.0f));

			textureRect.Texture = cursorIconsBank.Default;

			descriptionLabel.Text = "";

			if (rayInteractor.Interactable != null)
			{
				descriptionLabel.Text = rayInteractor.Interactable.InteractableDescription;

				if (rayInteractor.Interactable.InteractionType == InteractionTypeEnum.GRAB)
				{
					textureRect.Texture = cursorIconsBank.Grab;
				}
				if (rayInteractor.Interactable.InteractionType == InteractionTypeEnum.PICKUP)
				{
					textureRect.Texture = cursorIconsBank.PICKUP;
				}
				if (rayInteractor.Interactable.InteractionType == InteractionTypeEnum.INVENTORY_DRAG)
				{
					textureRect.Texture = cursorIconsBank.InventoryDrag;
				}
			}
		}
	}
}