using System;
using Godot;

namespace Game
{
	public partial class Client : Node
	{
		public bool Connected = false;

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

		public bool Join(string ipAddress = "localhost")
		{
			if (Connected) return false;

			var peer = new ENetMultiplayerPeer();
			var check = peer.CreateClient(ipAddress, Constants.MULTIPLAYER_PORT);
			if (check == Error.Ok)
			{
				Multiplayer.MultiplayerPeer = peer;
			}

			Multiplayer.ConnectedToServer += void () =>
			{
				Connected = true;
				GD.Print("CLIENT: connected to server!");

				locationLoader.RequestInstantiateServerScene();
				Action onLocLoaded = null;
				onLocLoaded = void () =>
				{
					playersManager.RequestPlayerSpawn();
					locationLoader.OnLocationLoaded -= onLocLoaded;
				};
				locationLoader.OnLocationLoaded += onLocLoaded;
			};
			Multiplayer.ServerDisconnected += void () =>
			{
				Connected = false;
				Input.MouseMode = Input.MouseModeEnum.Visible;
				GD.Print("CLIENT: server disconnected!");
			};
			return true;
		}
	}
}