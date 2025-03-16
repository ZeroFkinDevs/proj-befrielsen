using Godot;

namespace Game.UI
{
	public partial class ClientPanel : Control
	{
		[Export]
		public Client ClientNode;
		[Export]
		public LineEdit IpAddressInput;
		[Export]
		public Control HideMenu;
		[Export]
		public Label InfoLabel;
		[Export]
		public Button JoinButton;

		public override void _Ready()
		{
			JoinButton.Pressed += OnJoinButtonPressed;
		}

		public void OnJoinButtonPressed()
		{
			Join();
		}
		public void Join()
		{
			var ipAddress = IpAddressInput.Text;
			var res = ClientNode.Join(ipAddress);
			if (res) InfoLabel.Text = "Подключаемся к серверу...";

			ClientNode.Multiplayer.ConnectedToServer += void () =>
			{
				InfoLabel.Text = "Подключено!";
				HideMenu.Visible = false;
			};
			ClientNode.Multiplayer.ServerDisconnected += void () =>
			{
				InfoLabel.Text = "Отключено от сервера!";
				HideMenu.Visible = true;
			};
		}
	}
}