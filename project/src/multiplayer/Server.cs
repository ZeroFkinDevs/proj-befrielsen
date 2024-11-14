using Godot;

namespace Game
{
	public partial class Server : Node
	{
		public bool Started = false;

		[Export]
		public Node3D LocationContainer;

		public Location LocationInstance;

		public override void _EnterTree()
		{
			GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), this.GetPath());
		}

		public override void _Ready()
		{

		}
		public void SpawnLocation()
		{
			var locInstance = Global.Instance.LocationScene.Instantiate<Location>();
			LocationInstance = locInstance;
			LocationContainer.AddChild(locInstance);
		}

		public bool Host()
		{
			if (Started) return false;

			var peer = new ENetMultiplayerPeer();
			var check = peer.CreateServer(Constants.MULTIPLAYER_PORT);
			if (check == Error.Ok)
			{
				Multiplayer.MultiplayerPeer = peer;
				SpawnLocation();
				Started = true;
			}
			else
			{
				GetParent().QueueFree();
				return false;
			}
			Multiplayer.PeerConnected += void (long peerId) =>
			{
				GD.Print("SERVER: peer connected! " + peerId);
			};
			Multiplayer.PeerDisconnected += void (long peerId) =>
			{
				GD.Print("SERVER: peer disconnected: " + peerId);
			};
			return true;
		}
	}
}