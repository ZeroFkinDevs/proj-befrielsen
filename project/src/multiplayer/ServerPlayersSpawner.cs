using Godot;
using System;

namespace Game
{
	public partial class ServerPlayersSpawner : Node
	{
		[Export]
		public PackedScene PlayerScene;

		public override void _Ready()
		{
			Multiplayer.PeerConnected += SpawnPlayer;
		}

		public void SpawnPlayer(long peerId)
		{
			var player = PlayerScene.Instantiate<Node3D>();
			player.Name = peerId.ToString();
			AddChild(player);
		}
	}
}