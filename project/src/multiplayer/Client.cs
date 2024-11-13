using Godot;
using System;

namespace Game
{
	public partial class Client : Node
	{
		public override void _EnterTree()
		{
			GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), this.GetPath());
		}

		public override void _Ready()
		{
			Join();
		}

		public void Join()
		{
			var peer = new ENetMultiplayerPeer();
			var check = peer.CreateClient("localhost", Constants.MULTIPLAYER_PORT);
			if (check == Error.Ok)
			{
				Multiplayer.MultiplayerPeer = peer;
			}

			Multiplayer.ConnectedToServer += void () =>
			{
				GD.Print("CLIENT: connected to server!");
			};
			Multiplayer.ServerDisconnected += void () =>
			{
				GD.Print("CLIENT: server disconnected!");
				GetTree().Quit();
			};
		}
	}
}