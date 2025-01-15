using System;
using System.Linq;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class LocationLoader : Node3D
    {
        [Export]
        public TmpStorage tmpStorage;
        [Export]
        public Node3D LocationContainer;

        public Location LocationInstance;

        [Export]
        public PackedScene defaultScene;
        public event Action OnLocationLoaded;

        public string LocationName = "world0";
        public string SavesDirectory = "user://saves/";
        public string LocationSceneFilepath { get { return SavesDirectory + LocationName + ".tscn"; } }

        public override void _Ready()
        {
            MakeDirs();
        }
        private void MakeDirs()
        {
            DirAccess.MakeDirRecursiveAbsolute(SavesDirectory);
        }

        public void InstantiateDefaultScene()
        {
            InstantiateScene(defaultScene);
        }

        public PackedScene PackLocation()
        {
            var packedScene = new PackedScene();
            packedScene.Pack(LocationInstance);
            foreach (var node in LocationInstance.GetChildren())
            {
                // LocationInstance.SetEditableInstance(node, true);
            }
            return packedScene;
        }

        public void InstantiateScene(PackedScene scene)
        {
            var locInstance = scene.Instantiate<Location>();
            LocationInstance = locInstance;
            LocationContainer.AddChild(locInstance);
        }

        public void RequestInstantiateServerScene()
        {
            RpcId(1, MethodName.ServerSendBackCurrentLocation);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerSendBackCurrentLocation()
        {
            var peerId = Multiplayer.GetRemoteSenderId();
            var packedScene = PackLocation();

            tmpStorage.SendArrayOfResourcesToPeer(peerId,
                new Array<PackedScene> { packedScene },
                "server_location",
                this, MethodName.RecieveInstantiateServerScene,
                new Array<string> { });
        }
        public void RecieveInstantiateServerScene(Array<PackedScene> scenes, Array<string> args = null)
        {
            var scene = scenes[0];
            InstantiateScene(scene);
            OnLocationLoaded?.Invoke();
        }

        public void RequestSaveLocation()
        {
            RpcId(1, MethodName.ServerSaveLocation);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerSaveLocation()
        {
            var packedScene = PackLocation();
            ResourceSaver.Save(packedScene, LocationSceneFilepath);
        }

        public void LoadLocation(string filepath)
        {
            var packedScene = (PackedScene)ResourceLoader.Load(filepath, cacheMode: ResourceLoader.CacheMode.Ignore);
            var name = Utils.PathUtils.GetFileName(filepath);
            LocationName = name;
            InstantiateScene(packedScene);
        }

        public Array<string> GetSavedLocationsFiles()
        {
            var files = new Array<string>();
            foreach (var file in DirAccess.GetFilesAt(SavesDirectory))
            {
                files.Add(SavesDirectory + file);
            }
            return files;
        }
    }
}