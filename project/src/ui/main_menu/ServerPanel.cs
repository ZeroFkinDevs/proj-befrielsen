using Godot;

namespace Game.UI
{
	public partial class ServerPanel : Control
	{
		[Export]
		public Server ServerNode;
		[Export]
		public Label InfoLabel;

		public void _on_button_pressed()
		{
			var res = ServerNode.Host();
			if (res) InfoLabel.Text = "Сервер запущен";
			else InfoLabel.Text = "Не удалось запустить сервер, скорее всего он уже запущен";
		}
	}
}