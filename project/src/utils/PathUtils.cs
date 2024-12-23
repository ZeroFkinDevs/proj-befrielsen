using Godot;

namespace Game.Utils
{
    public static class PathUtils
    {
        public static string GetFileName(string filepath)
        {
            var name = filepath.Substring(filepath.LastIndexOf("/") + 1);
            name = name.Substring(0, name.LastIndexOf("."));
            return name;
        }
    }
}