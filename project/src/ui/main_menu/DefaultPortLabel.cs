using Godot;

namespace Game.UI
{
	public partial class DefaultPortLabel : Label
	{

		public override void _Ready()
		{
			Text = "дефолтный порт: " + Constants.MULTIPLAYER_PORT;
		}
	}
}