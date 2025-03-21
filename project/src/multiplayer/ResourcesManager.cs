using System;
using System.Linq;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class ResourcesManager : Node3D
    {
        public string SavesDir = "user://tmp/";
        public string FolderName = "";
        public string DirectoryPath { get { return SavesDir + FolderName + "/"; } }

        public Dictionary<string, Array<int>> ResourcesLoadState = new Dictionary<string, Array<int>>();
        public Array<string> LoadedResources = new Array<string>();
        public event Action<string> OnResourceLoaded;

        public static ResourcesManager CreateFromFolder(Node parentNode, string folderName)
        {
            var _resourcesManager = new ResourcesManager();
            _resourcesManager.FolderName = folderName;
            parentNode.AddChild(_resourcesManager);
            return _resourcesManager;
        }

        public void DownloadRemoteContent()
        {

        }

        public void BroadcastResource(Resource resource, bool unique)
        {

        }

        // DirAccess.MakeDirRecursiveAbsolute(SavesDirectory);
    }
}