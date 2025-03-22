using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class ResourcesManager : Node3D
    {
        public string VolumeName = "";
        public string VolumesDir = "user://tmp/";
        public string DirectoryPath { get { return VolumesDir + VolumeName + "/"; } }

        [Export]
        public Array<string> LoadedResourcesFiles = new Array<string>();

        public event Action<int> PeerDownloadedContent;
        public event Action<Array<string>> DownloadedResources;
        public event Action<string> ClientResourceSaved;

        public event Action<string> OnBeforeResourceSend;

        public ResourcesBroadcaster Broadcaster;

        public static ResourcesManager CreateFromFolder(Node parentNode, string folderName)
        {
            var _resourcesManager = new ResourcesManager();
            _resourcesManager.VolumeName = folderName;
            _resourcesManager.Name = "ResourcesManager";
            _resourcesManager.Ready += _resourcesManager.RequestRemoveUnusedVolumes;
            parentNode.AddChild(_resourcesManager);
            _resourcesManager.Broadcaster = ResourcesBroadcaster.CreateWithResourcesManager(_resourcesManager);
            return _resourcesManager;
        }

        // сохранения ресурса для серверной логики
        public string ServerRequestSaveResource(Resource resource, string name = null)
        {
            var resAccess = new ResourcesAccess();
            var (packedResource, filetype) = resAccess.PackResourceToString(resource);

            if (name == null) name = HashUtils.RandomMd5();
            if (name.Contains(filetype)) name = name.Replace(filetype, "");

            SaveResource(packedResource, name + filetype);
            var filename = name + filetype;

            return filename;
        }

        // основной метод
        // запрашиваем сохранение ресурса
        public async Task<string> RequestSaveResource(Resource resource, string name = null)
        {
            var resAccess = new ResourcesAccess();
            var (packedResource, filetype) = resAccess.PackResourceToString(resource);

            if (name == null) name = HashUtils.RandomMd5();
            if (name.Contains(filetype)) name = name.Replace(filetype, "");

            var filename = name + filetype;

            var task = new EventAwait<string>()
                .OnConnect(f => ClientResourceSaved += f)
                .OnDisconnect(f => ClientResourceSaved -= f)
                .WithCondition(fname => fname == filename)
                .Await();
            RpcId(1, MethodName.ServerSaveResource, packedResource, filetype, name);

            await task;

            return filename;
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ClientRecieveResourceLoaded(string name)
        {
            ClientResourceSaved?.Invoke(name);
        }

        // на серверной стороне сохраняем ресурс, и выполняем актуализацию для всех клиентов
        // name нужно указывать если необходимо понять что определенный ресурс загружен
        // TODO: сделать загрузку массива ресурсов
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public async void ServerSaveResource(string packedResource, string filetype, string name = null)
        {
            var peerId = Multiplayer.GetRemoteSenderId();
            if (name == "") name = null;
            if (name == null) name = HashUtils.RandomMd5();

            SaveResource(packedResource, name + filetype);
            await ServerSyncResources();

            RpcId(peerId, MethodName.ClientRecieveResourceLoaded, name + filetype);
        }

        // сохраняет ресурс и добавляет в список загруженных
        public void SaveResource(string packedResource, string filename)
        {
            var resAccess = new ResourcesAccess();
            DirAccess.MakeDirRecursiveAbsolute(DirectoryPath);
            resAccess.SaveResource(packedResource, DirectoryPath + filename);
            LoadedResourcesFiles.Add(filename);
        }

        public bool HasResource(string filename)
        {
            return LoadedResourcesFiles.Contains(filename);
        }

        public T LoadResource<T>(string filename) where T : Resource
        {
            return ResourceLoader.Load<T>(DirectoryPath + filename);
        }

        // актуализация ресурсов для всех клиентов
        public async Task ServerSyncResources()
        {
            var peers = new List<int>();
            foreach (var peer in Multiplayer.GetPeers())
            {
                if (peer == 1) continue;
                RpcId(peer, MethodName.RequestDownloadContent);
                peers.Add(peer);
            }
            await Task.WhenAll(peers.Select(peerId =>
            {
                return new EventAwait<int>()
                    .OnConnect(f => PeerDownloadedContent += f)
                    .OnDisconnect(f => PeerDownloadedContent -= f)
                    .WithCondition(id => id == peerId)
                    .Await();
            }));
        }

        // callback от клиента о том что синхронизация прошла
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerRecievePeerDownload()
        {
            var peerId = Multiplayer.GetRemoteSenderId();
            PeerDownloadedContent?.Invoke(peerId);
        }

        // для RPC
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RequestDownloadContent()
        {
            RequestAwaitDownloadContent();
        }

        // посылает серверу текущие загруженные ресурсы, чтобы тот отправил недостающие
        public async Task RequestAwaitDownloadContent()
        {
            RpcId(1, MethodName.ServerSendMissingContent, LoadedResourcesFiles);
            await new EventAwait<Array<string>>()
                .OnConnect(f => DownloadedResources += f)
                .OnDisconnect(f => DownloadedResources -= f)
                .Await();
            RpcId(1, MethodName.ServerRecievePeerDownload);
        }

        // отправляет клиенту недостающие ресурсы
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerSendMissingContent(Array<string> peerResources)
        {
            var peerId = Multiplayer.GetRemoteSenderId();
            Array<string> missingResources = new Array<string>();
            Array<string> missingFilenames = new Array<string>();
            var resAccess = new ResourcesAccess();

            foreach (var file in LoadedResourcesFiles)
            {
                if (!peerResources.Contains(file))
                {
                    OnBeforeResourceSend?.Invoke(file);
                    missingResources.Add(resAccess.ReadResource(DirectoryPath + file));
                    missingFilenames.Add(file);
                }
            }

            RpcId(peerId, MethodName.RecieveResources, missingResources, missingFilenames);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveResources(Array<string> packedResources, Array<string> filenames)
        {
            for (int i = 0; i < packedResources.Count; i++)
            {
                SaveResource(packedResources[i], filenames[i]);
            }
            DownloadedResources?.Invoke(filenames);
        }

        public void RemoveGarbage()
        {
            foreach (var filename in LoadedResourcesFiles.ToList())
            {
                var res = LoadResource<Resource>(filename);
                if (res.GetReferenceCount() <= 1)
                {
                    if (FileAccess.FileExists(DirectoryPath + filename))
                    {
                        var dirAccess = DirAccess.Open(DirectoryPath);
                        dirAccess.Remove(filename);
                    }
                }
            }
        }

        public Array<string> GetVolumes()
        {
            var dir = DirAccess.Open(VolumesDir);
            return new Array<string>(dir.GetDirectories());
        }
        public void RequestRemoveUnusedVolumes()
        {
            RpcId(1, MethodName.ServerRequestRemoveVolumes, GetVolumes());
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerRequestRemoveVolumes(Array<string> volumes)
        {
            var peerId = Multiplayer.GetRemoteSenderId();
            volumes.Remove(VolumeName);
            RpcId(peerId, MethodName.RecieveVolumesToRemove, volumes);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveVolumesToRemove(Array<string> volumes)
        {
            var dir = DirAccess.Open(VolumesDir);
            foreach (var volume in volumes)
            {
                var volumeDir = DirAccess.Open(VolumesDir + volume);
                foreach (var file in volumeDir.GetFiles())
                {
                    volumeDir.Remove(file);
                }
                dir.Remove(volume);
            }
        }
    }
}