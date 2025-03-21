using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Game
{
	public partial class GirlLockedBrain : NpcBrain, IGirlBrain, IInteractable
	{
		[Export]
		public BodyDetectionArea playerCheckArea;
		public BodyFetcher<Player> playerFetcher = new BodyFetcher<Player>();
		[Export]
		public NodeToBoneConnector nodeToBoneConnector;
		Player player;

		public bool ReadyToTalk = false;

		public ITalkableModel talkableModel => npc.CharacterModel as ITalkableModel;

		public InteractionTypeEnum InteractionType => InteractionTypeEnum.TOUCH;

		public string InteractableDescription => "talk";

		public GirlMood mood { get; set; }

		public Prop grabbedProp = null;

		public override void _EnterTree()
		{
			base._EnterTree();
			mood = GirlMood.GOOD;
		}

		public override void _Ready()
		{
			playerCheckArea.AddFetcher(playerFetcher);
			playerFetcher.OnFilled += OnPlayerAreaFilled;
			playerFetcher.OnEmptied += OnPlayerAreaEmptied;

			if (talkableModel != null) talkableModel.Alone();
		}

		public void OnPlayerAreaFilled()
		{
			OnGreet();
		}
		public void OnPlayerAreaEmptied()
		{
			BeforeLeave();
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
			if (playerFetcher.NearestObject == null) return;
			if (talkableModel == null) return;

			player = playerFetcher.NearestObject;
			talkableModel.SetLookTarget(player.Camera.GlobalPosition);

			var distance = player.GlobalPosition.DistanceTo(npc.GlobalPosition);
			if (distance > 2.5f)
			{

			}

			CheckProp();
		}

		public void CheckProp()
		{
			if (grabbedProp != null)
			{
				if (!IsInstanceValid(grabbedProp)) RemoveProp();
			}
		}

		public void Interact(IUser user)
		{

		}

		public void RemoveProp()
		{
			if (grabbedProp != null && IsInstanceValid(grabbedProp))
			{
				grabbedProp.QueueFree();
			}
			nodeToBoneConnector.skeleton = npc.CharacterModel.skeleton3D;
			nodeToBoneConnector.node = null;
			grabbedProp = null;
			talkableModel.Release();
		}

		public void TakeProp(Prop prop)
		{
			grabbedProp = prop;
			nodeToBoneConnector.skeleton = npc.CharacterModel.skeleton3D;
			nodeToBoneConnector.node = prop;
			prop.GravityScale = 0.0f;

			nodeToBoneConnector.Sync();
			if (prop.ModelSmoothConnector.Object != null)
			{
				prop.ModelSmoothConnector.NoSmooth = true;
				prop.ModelSmoothConnector.Teleport();
			}
		}

		public async Task RequestToGiveProp(ItemResource propItem)
		{
			await talkableModel.Give(() =>
			{
				if (Multiplayer.IsServer())
				{
					npc.objectInstantiator.RequestInstantiateProp(propItem, this, MethodName.TakeProp);
				}
			});
		}

		public async Task RequestToGiveProp(string itemResourcePath)
		{
			var itemRes = ResourceLoader.Load<ItemResource>(itemResourcePath);
			await RequestToGiveProp(itemRes);
		}
	}
}