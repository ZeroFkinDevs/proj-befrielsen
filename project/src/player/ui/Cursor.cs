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

		public override void _Process(double delta)
		{
			GlobalPosition = GetGlobalMousePosition();
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				GlobalPosition = GetViewportRect().Size / 2.0f;
			}

			textureRect.Texture = cursorIconsBank.Default;

			if (rayInteractor.Interactable != null)
			{
				if (rayInteractor.Interactable.InteractionType == InteractionTypeEnum.GRAB)
				{
					textureRect.Texture = cursorIconsBank.Grab;
				}
				if (rayInteractor.Interactable.InteractionType == InteractionTypeEnum.PICKUP)
				{
					textureRect.Texture = cursorIconsBank.PICKUP;
				}
			}
		}
	}
}