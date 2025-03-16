using System.Linq;
using Godot;

namespace Game
{
    public class SettingsRecord
    {
        public SettingsRecord()
        {
            DEBUG = false;
            var args = OS.GetCmdlineArgs();
            if (args.Contains("--DEBUG"))
            {
                DEBUG = true;
            }
        }
        public bool DEBUG { get; }
    }

    public class Settings
    {
        public SettingsRecord record { get; }
        public Settings()
        {
            record = new SettingsRecord();
        }
    }

    public class GlobalSettings
    {
        private static Settings _settings = null;
        public static Settings settings
        {
            get
            {
                if (_settings == null) _settings = new Settings();
                return _settings;
            }
        }
        public static SettingsRecord record => settings.record;
    }
}