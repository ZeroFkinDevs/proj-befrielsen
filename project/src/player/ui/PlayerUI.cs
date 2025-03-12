using System;
using Godot;

namespace Game
{
	public partial class PlayerUI : Control
	{
		[Export]
		public Player player;

		public override void _Ready()
		{
			if (player.IsMultiplayerAuthority())
			{
				CallDeferred(MethodName.Init);
			}
		}

		public void Init()
		{
			Reparent(GetNode("/root/Main/UI"));
		}
	}
}