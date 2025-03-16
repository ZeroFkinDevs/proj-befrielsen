using Godot;

namespace Game.UI
{
    public partial class MainMenu : Control
    {
        [Export]
        public ClientPanel clientPanel;
        [Export]
        public ServerPanel serverPanel;

        public override void _Ready()
        {
            if (GlobalSettings.record.DEBUG)
            {
                if (!serverPanel.Host(null))
                {
                    clientPanel.Join();
                }
            }
        }
    }
}