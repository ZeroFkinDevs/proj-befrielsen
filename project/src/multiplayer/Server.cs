using Godot;

namespace Game
{
	public partial class Server : Node
	{
		public bool Started = false;

		[Export]
		public LocationLoader locationLoader;
		[Export]
		public PlayersManager playersManager;

		public override void _EnterTree()
		{
			GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), this.GetPath());
		}

		public override void _Ready()
		{

		}

		public bool Host(string saveFilePath)
		{
			if (Started) return false;

			var peer = new ENetMultiplayerPeer();
			var check = peer.CreateServer(Constants.MULTIPLAYER_PORT);
			if (check == Error.Ok)
			{
				Multiplayer.MultiplayerPeer = peer;
				if (saveFilePath == null)
				{
					locationLoader.InstantiateDefaultScene();
				}
				else
				{
					locationLoader.LoadLocation(saveFilePath);
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