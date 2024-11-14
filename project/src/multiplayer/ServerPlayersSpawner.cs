using Godot;

namespace Game
{
	public partial class ServerPlayersSpawner : Node
	{
		[Export]
		public PackedScene PlayerScene;
		[Export]
		public Server ServerNode;

		public override void _Ready()
		{
			Multiplayer.PeerConnected += SpawnPlayer;
		}

		public void SpawnPlayer(long peerId)
		{
			var player = PlayerScene.Instantiate<Node3D>();
			player.Name = peerId.ToString();
			var spawners = ServerNode.LocationInstance.PlayerSpawners;
			AddChild(player);
			if (spawners.Count > 0)
			{
				var pos = spawners[0].GlobalPosition;
				player.RpcId(peerId, Player.MethodName.RecievePosition, pos);
			}
		}
	}
}