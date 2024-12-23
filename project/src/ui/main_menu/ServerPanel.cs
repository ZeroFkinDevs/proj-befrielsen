using Godot;

namespace Game.UI
{
	public partial class ServerPanel : Control
	{
		[Export]
		public Server ServerNode;
		[Export]
		public Label InfoLabel;
		[Export]
		public Control LoadButtonsContainer;

		public void _on_button_pressed()
		{
			Host(null);
		}

		public void Host(string saveFilePath)
		{
			var res = ServerNode.Host(saveFilePath);
			if (res) InfoLabel.Text = "Сервер запущен";
			else InfoLabel.Text = "Не удалось запустить сервер, скорее всего он уже запущен";
		}

		public override void _Ready()
		{
			base._Ready();
			var files = ServerNode.locationLoader.GetSavedLocationsFiles();
			if (files.Count > 0)
			{
				var label = new Label();
				label.Text = "или загрузить:";
				LoadButtonsContainer.AddChild(label);
			}
			foreach (var filepath in files)
			{
				var loadButton = new Button();
				var name = Utils.PathUtils.GetFileName(filepath);
				loadButton.Text = name;
				LoadButtonsContainer.AddChild(loadButton);
				loadButton.Pressed += void () =>
				{
					Host(filepath);
				};
			}
		}
	}
}