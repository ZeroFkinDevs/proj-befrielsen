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

			if (rayInteractor.CurrentInteractionType != InteractionTypeEnum.NONE)
			{
				descriptionLabel.Text = rayInteractor.CurrentInteractionDescription;

				if (rayInteractor.CurrentInteractionType == InteractionTypeEnum.GRAB)
				{
					textureRect.Texture = cursorIconsBank.Grab;
				}
				if (rayInteractor.CurrentInteractionType == InteractionTypeEnum.PICKUP)
				{
					textureRect.Texture = cursorIconsBank.PICKUP;
				}
				if (rayInteractor.CurrentInteractionType == InteractionTypeEnum.INVENTORY_DRAG)
				{
					textureRect.Texture = cursorIconsBank.InventoryDrag;
				}
				if (rayInteractor.CurrentInteractionType == InteractionTypeEnum.APPLY)
				{
					textureRect.Texture = cursorIconsBank.Apply;
				}
				if (rayInteractor.CurrentInteractionType == InteractionTypeEnum.TOUCH)
				{
					textureRect.Texture = cursorIconsBank.Touch;
				}
			}
		}
	}
}