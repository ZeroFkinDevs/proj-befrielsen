using Godot;
using Godot.Collections;

namespace Game
{
	public partial class GirlLockedBrain : NpcBrain, IInteractable
	{
		[Export]
		public Area3D playerCheckArea;
		Player player;

		public bool ReadyToTalk = false;

		public ITalkableModel talkableModel => npc.CharacterModel as ITalkableModel;

		public InteractionTypeEnum InteractionType => InteractionTypeEnum.TOUCH;

		public string InteractableDescription => "talk";

		public override void _Ready()
		{
			playerCheckArea.BodyEntered += OnBodyEntered;
			playerCheckArea.BodyExited += OnBodyExited;
			if (talkableModel != null) talkableModel.Alone();
		}

		public void OnBodyEntered(Node3D body)
		{
			if (body is Player _pl)
			{
				player = _pl;
				OnGreet();
			}
		}
		public void OnBodyExited(Node3D body)
		{
			if (body is Player _pl)
			{
				BeforeLeave();
				player = null;
			}
		}

		public void OnGreet()
		{
			if (ReadyToTalk) return;
			ReadyToTalk = true;
			talkableModel?.Greet();
		}
		public void BeforeLeave()
		{
			if (!ReadyToTalk) return;
			ReadyToTalk = false;
			talkableModel?.Leave();
		}

		public override void _Process(double delta)
		{
			if (!IsActive) return;
			if (player == null) return;
			if (talkableModel == null) return;
			talkableModel.SetLookTarget(player.Camera.GlobalPosition);

			var distance = player.GlobalPosition.DistanceTo(npc.GlobalPosition);
			if (distance > 2.5f)
			{

			}
		}

		public void Interact(IUser user)
		{
			if (npc.Dialogue != null)
			{
				npc.Dialogue.Goto("_");
			}
		}
	}
}