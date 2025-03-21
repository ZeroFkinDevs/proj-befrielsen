using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class ResourcesBroadcaster : Node3D
    {
        public ResourcesManager resourcesManager;

        public static ResourcesBroadcaster CreateWithResourcesManager(ResourcesManager resourcesManager)
        {
            ResourcesBroadcaster broadcaster = new ResourcesBroadcaster();
            broadcaster.resourcesManager = resourcesManager;
            broadcaster.Name = "ResourcesBroadcaster";
            resourcesManager.AddChild(broadcaster);
            return broadcaster;
        }

        public async void BroadcastArrayOfResources<[MustBeVariant] T>(Array<T> resVariant, Node3D node, StringName nodeRecieveMethod, Array<string> recieveArgs = null)
        {
            var savedResourcesPaths = new Array<string>();
            foreach (var item in resVariant)
            {
                if (item is Resource res)
                {
                    var filename = "";

                    if (Multiplayer.IsServer())
                    {
                        filename = resourcesManager.ServerRequestSaveResource(res);
                    }
                    else
                    {
                        filename = await resourcesManager.RequestSaveResource(res);
                    }
                    savedResourcesPaths.Add(filename);
                }
            }
            RpcId(1, MethodName.ServerBroadcastResources, savedResourcesPaths, node.GetMultiplayerPath(), nodeRecieveMethod, recieveArgs);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerBroadcastResources(Array<string> resPaths, string nodePath, StringName nodeRecieveMethod, Array<string> recieveArgs)
        {
            Rpc(MethodName.RecieveResources, resPaths, nodePath, nodeRecieveMethod, recieveArgs);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveResources(Array<string> resPaths, string nodePath, StringName nodeRecieveMethod, Array<string> recieveArgs)
        {
            var resArray = new Array<Resource>();
            foreach (var filepath in resPaths)
            {
                var res = resourcesManager.LoadResource<Resource>(filepath);
                resArray.Add(res);
            }

            var node = this.GetMultiplayerNode<Node3D>(nodePath);
            node.Call(nodeRecieveMethod, resArray, recieveArgs);
        }
    }
}