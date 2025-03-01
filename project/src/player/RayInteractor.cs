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
		public IToolInteractable ToolInteractable = null;

		public InteractionTypeEnum CurrentInteractionType = InteractionTypeEnum.NONE;
		public string CurrentInteractionDescription;

		public ToolsManager toolsManager
		{
			get
			{
				if (player == null) return null;
				return player.inventoryManager.toolsManager;
			}
		}

		public override void _Process(double delta)
		{
			if (!player.Controllable) return;

			Interactable = null;
			ToolInteractable = null;
			CurrentInteractionType = InteractionTypeEnum.NONE;
			CurrentInteractionDescription = "";

			if (!Enabled) return;

			if (UseRaycast.IsColliding())
			{
				CheckCurrentInteractable();
			}

			TryToUseCurrentInteractable();
		}

		public void CheckCurrentInteractable()
		{
			var collider = UseRaycast.GetCollider();
			if (collider is IToolInteractable toolInteractable && CurrentInteractionType == InteractionTypeEnum.NONE)
			{
				if (toolsManager.HasActiveTool)
				{
					var interType = toolInteractable.GetInteractionType(toolsManager.CurrentITool, toolsManager.CurrentToolItemStack);
					if (interType != InteractionTypeEnum.NONE)
					{
						ToolInteractable = toolInteractable;
						CurrentInteractionDescription = toolInteractable.GetInteractableDescription(toolsManager.CurrentITool, toolsManager.CurrentToolItemStack);
						CurrentInteractionType = interType;
					}
				}
			}
			if (collider is IInteractable usable && CurrentInteractionType == InteractionTypeEnum.NONE)
			{
				var interType = usable.InteractionType;
				if (usable.InteractionType != InteractionTypeEnum.NONE)
				{
					Interactable = usable;
					CurrentInteractionDescription = usable.InteractableDescription;
					CurrentInteractionType = interType;
				}
			}
		}
		public void TryToUseCurrentInteractable()
		{
			if (ToolInteractable != null && Input.IsActionJustPressed("use"))
			{
				if (CurrentInteractionType == InteractionTypeEnum.TOUCH)
				{
					ToolInteractable.Interact(toolsManager.CurrentITool, toolsManager.CurrentToolItemStack);
				}
			}
			else if (Interactable != null && Input.IsActionJustPressed("use"))
			{
				if (Interactable.InteractionType == InteractionTypeEnum.GRAB)
				{
					grabber.Grab(Interactable);
				}
				else if (Interactable.InteractionType == InteractionTypeEnum.PICKUP)
				{
					Interactable.Interact(player);
				}
				else if (Interactable.InteractionType == InteractionTypeEnum.TOUCH)
				{
					Interactable.Interact(player);
				}
			}
			else if (Interactable != null && Input.IsActionJustPressed("fire"))
			{
				if (Interactable.InteractionType == InteractionTypeEnum.INVENTORY_DRAG)
				{
					Interactable.Interact(player);
				}
			}
		}
	}
}