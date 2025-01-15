using Godot;

namespace Game
{
	public partial class PlayersManager : Node
	{
		[Export]
		public PackedScene PlayerScene;
		[Export]
		public Server ServerNode;

		public override void _Ready()
		{

		}

		public void RequestPlayerSpawn()
		{
			RpcId(1, MethodName.SpawnPeerPlayer);
		}

		[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
		public void SpawnPeerPlayer()
		{
			var peerId = Multiplayer.GetRemoteSenderId();
			var player = PlayerScene.Instantiate<Node3D>();
			player.Name = peerId.ToString();
			var spawners = ServerNode.locationLoader.LocationInstance.PlayerSpawners;
			AddChild(player);
			if (spawners.Count > 0)
			{
				var pos = spawners[0].GlobalPosition;
				player.RpcId(peerId, Player.MethodName.RecievePosition, pos);
			}
		}

		public void PlayerDespawn(long id)
		{
			var player = GetNode<Player>(id.ToString());
			if (player != null)
			{
				player.Despawn();
			}
		}
	}
}