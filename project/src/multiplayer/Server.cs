using Godot;
using System;

namespace Game
{
	public partial class Server : Node
	{
		public override void _EnterTree()
		{
			GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), this.GetPath());
		}

		public override void _Ready()
		{
			Host();
		}

		public void Host()
		{
			var peer = new ENetMultiplayerPeer();
			var check = peer.CreateServer(Constants.MULTIPLAYER_PORT);
			if (check == Error.Ok)
			{
				Multiplayer.MultiplayerPeer = peer;
			}
			else
			{
				GetParent().QueueFree();
				return;
			}
			Multiplayer.PeerConnected += void (long peerId) =>
			{
				GD.Print("SERVER: peer connected! " + peerId);
			};
			Multiplayer.PeerDisconnected += void (long peerId) =>
			{
				GD.Print("SERVER: peer disconnected: " + peerId);
			};
		}
	}
}