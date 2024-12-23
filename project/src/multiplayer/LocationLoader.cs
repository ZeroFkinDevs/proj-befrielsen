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

        public override void _Ready()
        {

        }

        public void InstantiateDefaultScene()
        {
            InstantiateScene(defaultScene);
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
            var packedScene = new PackedScene();
            packedScene.Pack(LocationInstance);

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
    }
}