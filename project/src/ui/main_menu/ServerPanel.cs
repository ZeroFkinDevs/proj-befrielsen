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
		[Export]
		public Button NewWorldButton;

		public void OnNewWorldButtonPressed()
		{
			Host(null);
		}

		public bool Host(string saveFilePath)
		{
			var res = ServerNode.Host(saveFilePath);
			if (res) InfoLabel.Text = "Сервер запущен";
			else InfoLabel.Text = "Не удалось запустить сервер, скорее всего он уже запущен";
			return res;
		}

		public override void _Ready()
		{
			base._Ready();

			NewWorldButton.Pressed += OnNewWorldButtonPressed;

			SetupSavesFiles();
		}
		public void SetupSavesFiles()
		{
			var files = ServerNode.worldContainer.GetWorldsList();
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