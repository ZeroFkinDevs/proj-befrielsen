using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class WorldContainer : Node3D
    {
        private ResourcesManager _resourcesManager;
        public ResourcesManager resourcesManager => _resourcesManager;

        [Export]
        public Node3D LocationContainer;

        public Location LocationInstance;

        [Export]
        public PackedScene defaultScene;

        public string WorldName = "";
        private string worldFileName = "world.tscn";

        public async Task LoadWorldFromFolder(string worldFolderName)
        {
            var parts = worldFolderName.Split("-").ToList();
            parts.RemoveAt(parts.Count - 1);
            WorldName = String.Join("-", parts);
            _resourcesManager = ResourcesManager.CreateFromFolder(this, worldFolderName);

            if (Multiplayer.IsServer())
            {
                _resourcesManager.OnBeforeResourceSend += (string res) =>
                {
                    if (res == worldFileName) WriteLocation();
                };
                if (!_resourcesManager.HasResource(worldFileName))
                {
                    _resourcesManager.ServerRequestSaveResource(defaultScene, worldFileName);
                }
            }
            else
            {
                await _resourcesManager.RequestAwaitDownloadContent();
            }

            var scene = _resourcesManager.LoadResource<PackedScene>(worldFileName);
            LocationInstance = scene.Instantiate<Location>();
            LocationContainer.AddChild(LocationInstance);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void SaveWorld()
        {
            WriteLocation();
        }

        public void RequestSaveWorld()
        {
            RpcId(1, MethodName.SaveWorld);
        }

        public void CreateWorld(string worldName)
        {
            WorldName = worldName;
            string folderName = WorldName + "-" + HashUtils.RandomMd5();
            LoadWorldFromFolder(folderName);
        }

        public void WriteLocation()
        {
            var scenePack = new PackedScene();
            scenePack.Pack(LocationInstance);
            _resourcesManager.ServerRequestSaveResource(scenePack, worldFileName);
        }

        public void LoadWorld(string worldName)
        {
            throw new NotImplementedException(); // TODO:
        }

        public Array<string> GetWorldsList()
        {
            return new Array<string>(); // TODO:
        }

        public bool HasWorld(string worldName)
        {
            return false; // TODO:
        }


        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RequestServerFolderName()
        {
            RpcId(Multiplayer.GetRemoteSenderId(), MethodName.RecieveServerFolderName, _resourcesManager.VolumeName);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveServerFolderName(string name)
        {
            OnFolderNameRecieved?.Invoke(name);
        }
        public event Action<string> OnFolderNameRecieved;
        public async Task<string> FetchServerFolderName()
        {
            RpcId(1, MethodName.RequestServerFolderName);
            return await new EventAwait<string>()
                .OnConnect(f => OnFolderNameRecieved += f)
                .OnDisconnect(f => OnFolderNameRecieved -= f)
                .Await();
        }

        public async Task DownloadWorld()
        {
            var folderName = await FetchServerFolderName();
            await LoadWorldFromFolder(folderName);
        }
    }
}