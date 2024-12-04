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

        public void BroadcastArrayOfResources<[MustBeVariant] T>(Array<T> resVariant, string tag, Node3D node, StringName nodeRecieveMethod, Array<string> recieveArgs = null)
        {
            var res = Variant.From(resVariant).AsGodotArray<Resource>();

            var packedStacks = new Array<string>();
            foreach (var stack in res)
            {
                var fileType = ".tres";
                if (stack is PackedScene) fileType = ".tscn";
                var tmpItemResPath = DirectoryPath + tag + fileType;
                var result = ResourceSaver.Save(stack, tmpItemResPath);
                if (result == Error.Ok)
                {
                    var file = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Read);
                    var text = file.GetAsText();
                    file.Close();
                    packedStacks.Add(text);
                }
            }

            RpcId(1, MethodName.ServerBroadcastResources, packedStacks, tag, node.GetPath(), nodeRecieveMethod, recieveArgs);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerBroadcastResources(Array<string> packedRes, string tag, string nodePath, StringName nodeRecieveMethod, Array<string> recieveArgs)
        {
            Rpc(MethodName.RecieveResources, packedRes, tag, nodePath, nodeRecieveMethod, recieveArgs);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveResources(Array<string> packedRes, string tag, string nodePath, StringName nodeRecieveMethod, Array<string> recieveArgs)
        {
            var fileType = ".tres";
            if (FileAccess.FileExists(DirectoryPath + tag + ".tscn")) fileType = ".tscn";
            var tmpItemResPath = DirectoryPath + tag + fileType;

            var stacks = new Array<Resource>();
            foreach (var packedStack in packedRes)
            {
                var text = packedStack;
                var file = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Write);
                file.Seek(0);
                file.StoreString(text);
                file.Close();
                var stack = ResourceLoader.Load(tmpItemResPath);
                stacks.Add(stack);
            }

            var node = this.GetMultiplayerNode<Node3D>(nodePath);
            node.Call(nodeRecieveMethod, stacks, recieveArgs);

            if (LoadedResources.IndexOf(tmpItemResPath) != -1)
            {
                InvokeResourceLoaded(tmpItemResPath);
            }
            else
            {
                RpcId(1, MethodName.ServerRecievePeerResourceState, tmpItemResPath);
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