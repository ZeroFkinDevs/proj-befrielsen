using Godot;

namespace Game
{
	public partial class Server : Node
	{
		public bool Started = false;

		[Export]
		public WorldContainer worldContainer;
		[Export]
		public PlayersManager playersManager;

		public override void _EnterTree()
		{
			GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), this.GetPath());
		}

		public override void _Ready()
		{

		}

		public bool Host(string worldName = null, bool createNew = false)
		{
			if (Started) return false;

			var peer = new ENetMultiplayerPeer();
			var check = peer.CreateServer(Constants.MULTIPLAYER_PORT);
			if (check == Error.Ok)
			{
				Multiplayer.MultiplayerPeer = peer;
				if (worldName != null && worldContainer.HasWorld(worldName) && !createNew)
				{
					worldContainer.LoadWorld(worldName);
				}
				if (createNew)
				{
					if (worldName == null) worldName = "new world";
					worldContainer.CreateWorld(worldName);
				}
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
				playersManager.PlayerDespawn(peerId);
			};
			return true;
		}
	}
}