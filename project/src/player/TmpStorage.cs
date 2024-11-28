using System;
using Godot;

namespace Game
{
    public partial class TmpStorage : Node3D
    {
        public string TmpDirectory = "user://tmp/";
        public string DirectoryPath { get { return TmpDirectory + "id" + GetMultiplayerAuthority().ToString() + "/"; } }

        public override void _Ready()
        {
            MakeDirs();
        }
        private void MakeDirs()
        {
            DirAccess.MakeDirRecursiveAbsolute(DirectoryPath);
        }
        public void BroadcastArrayOfResources<[MustBeVariant] T>(Godot.Collections.Array<T> resVariant, string tag, Node3D node, StringName nodeRecieveMethod)
        {
            var res = Variant.From(resVariant).AsGodotArray<Resource>();

            var tmpItemResPath = DirectoryPath + tag + ".tres";
            var packedStacks = new Godot.Collections.Array<string>();
            foreach (var stack in res)
            {
                var result = ResourceSaver.Save(stack, tmpItemResPath);
                if (result == Error.Ok)
                {
                    var file = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Read);
                    var text = file.GetAsText();
                    file.Close();
                    packedStacks.Add(text);
                }
            }

            RpcId(1, MethodName.ServerBroadcastResources, packedStacks, tag, node.GetPath(), nodeRecieveMethod);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerBroadcastResources(Godot.Collections.Array<string> packedRes, string tag, string nodePath, StringName nodeRecieveMethod)
        {
            Rpc(MethodName.RecieveResources, packedRes, tag, nodePath, nodeRecieveMethod);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveResources(Godot.Collections.Array<string> packedRes, string tag, string nodePath, StringName nodeRecieveMethod)
        {
            var tmpItemResPath = DirectoryPath + tag + ".tres";
            var stacks = new Godot.Collections.Array<Resource>();
            foreach (var packedStack in packedRes)
            {
                var text = packedStack;
                var file = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Write);
                file.Seek(0);
                file.StoreString(text);
                file.Close();
                var stack = ResourceLoader.Load(tmpItemResPath).Duplicate();
                stacks.Add(stack);
            }

            GetNode(nodePath).Call(nodeRecieveMethod, stacks);
        }
    }
}