using System;
using System.Linq;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class TmpStorage : Node3D
    {
        public string TmpDirectory = "user://tmp/";
        public string DirectoryPath { get { return TmpDirectory + "id" + GetMultiplayerAuthority().ToString() + "/"; } }
        public Dictionary<string, Array<int>> ResourcesLoadState = new Dictionary<string, Array<int>>();

        public Array<string> LoadedResources = new Array<string>();

        public event Action<string> OnResourceLoaded;

        public override void _Ready()
        {
            MakeDirs();
        }
        private void MakeDirs()
        {
            DirAccess.MakeDirRecursiveAbsolute(DirectoryPath);
        }

        public (Array<string>, string) ConvertResourceToString<[MustBeVariant] T>(Array<T> resVariant, string tag)
        {
            var res = Variant.From(resVariant).AsGodotArray<Resource>();

            var packedResources = new Array<string>();
            int i = 0;
            var fileType = ".tres";
            foreach (var stack in res)
            {
                if (stack is PackedScene) fileType = ".tscn";
                var tmpItemResPath = DirectoryPath + tag + "_" + i + "-outcoming" + fileType;
                var result = ResourceSaver.Save(stack, tmpItemResPath);
                if (result == Error.Ok)
                {
                    var file = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Read);
                    var text = file.GetAsText();
                    file.Close();
                    packedResources.Add(text);
                }
                i += 1;
            }
            return (packedResources, fileType);
        }

        public void SendArrayOfResourcesToPeer<[MustBeVariant] T>(int peerId, Array<T> resVariant, string tag, Node3D node, StringName nodeRecieveMethod, Array<string> recieveArgs = null)
        {
            var (packedResources, fileType) = ConvertResourceToString(resVariant, tag);
            RpcId(peerId, MethodName.RecieveResources, packedResources, tag, fileType, node.GetPath(), nodeRecieveMethod, recieveArgs);
        }

        public void BroadcastArrayOfResources<[MustBeVariant] T>(Array<T> resVariant, string tag, Node3D node, StringName nodeRecieveMethod, Array<string> recieveArgs = null)
        {
            var (packedResources, fileType) = ConvertResourceToString(resVariant, tag);
            RpcId(1, MethodName.ServerBroadcastResources, packedResources, tag, fileType, node.GetPath(), nodeRecieveMethod, recieveArgs);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerBroadcastResources(Array<string> packedRes, string tag, string fileType, string nodePath, StringName nodeRecieveMethod, Array<string> recieveArgs)
        {
            Rpc(MethodName.RecieveResources, packedRes, tag, fileType, nodePath, nodeRecieveMethod, recieveArgs);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveResources(Array<string> packedRes, string tag, string fileType, string nodePath, StringName nodeRecieveMethod, Array<string> recieveArgs)
        {
            var tmpFirstItemResPath = DirectoryPath + tag + "_" + 0 + fileType;

            var stacks = new Array<Resource>();
            int i = 0;
            foreach (var packedStack in packedRes)
            {
                var tmpItemResPath = DirectoryPath + tag + "_" + i + fileType;
                var text = packedStack;

                var existText = "";
                if (FileAccess.FileExists(tmpItemResPath))
                {
                    var fileread = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Read);
                    existText = fileread.GetAsText();
                    fileread.Close();
                }
                if (existText != text)
                {
                    var file = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Write);
                    file.Seek(0);
                    file.StoreString(text);
                    file.Close();
                }

                var stack = ResourceLoader.Load(tmpItemResPath, cacheMode: ResourceLoader.CacheMode.Ignore);
                stack.ResourcePath = tmpItemResPath;
                stacks.Add(stack);
                i += 1;
            }

            var node = this.GetMultiplayerNode<Node3D>(nodePath);
            node.Call(nodeRecieveMethod, stacks, recieveArgs);

            if (LoadedResources.IndexOf(tmpFirstItemResPath) != -1)
            {
                InvokeResourceLoaded(tmpFirstItemResPath);
            }
            else
            {
                RpcId(1, MethodName.ServerRecievePeerResourceState, tmpFirstItemResPath);
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerRecievePeerResourceState(string resPath)
        {
            if (!ResourcesLoadState.ContainsKey(resPath))
            {
                ResourcesLoadState[resPath] = new Array<int>();
            }
            var peers = Multiplayer.GetPeers();
            var peerId = Multiplayer.GetRemoteSenderId();
            var arr = ResourcesLoadState[resPath];
            if (arr.IndexOf(peerId) == -1)
            {
                arr.Add(peerId);
                if (peers.Length == arr.Count)
                {
                    Rpc(MethodName.InvokeResourceLoaded, resPath);
                }
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void InvokeResourceLoaded(string resPath)
        {
            if (LoadedResources.IndexOf(resPath) == -1)
            {
                LoadedResources.Add(resPath);
            }
            OnResourceLoaded?.Invoke(resPath);
        }
    }
}