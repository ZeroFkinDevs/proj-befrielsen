using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public interface IWorldContainer
    {
        public void LoadWorldFromFolder(string worldFolderName);
        public void LoadWorld(string worldName);
        public Array<string> GetWorldsList();
        public Task DownloadWorld();
    }

    public partial class WorldContainer : Node3D, IWorldContainer
    {
        private ResourcesManager _resourcesManager;
        public ResourcesManager resourcesManager => _resourcesManager;

        [Export]
        public Node3D LocationContainer;

        public Location LocationInstance;

        [Export]
        public PackedScene defaultScene;

        public string WorldName = "";

        public void LoadWorldFromFolder(string worldFolderName)
        {
            WorldName = worldFolderName.Split("-")[0];
            _resourcesManager = ResourcesManager.CreateFromFolder(this, worldFolderName);
            // 
        }

        public void SaveWorld()
        {
            WriteLocation();
            // 
        }
        public void RequestSaveWorld()
        {

        }

        public void CreateWorld(string worldName)
        {
            WorldName = worldName;
            string folderName = WorldName + "-" + HashUtils.RandomMd5();
            LoadWorldFromFolder(folderName);
        }

        public void WriteLocation()
        {

        }

        public void LoadWorld(string worldName)
        {
            throw new NotImplementedException();
        }

        public Array<string> GetWorldsList()
        {
            return new Array<string>();
        }

        public Task DownloadWorld()
        {
            throw new NotImplementedException();
        }

    }
}