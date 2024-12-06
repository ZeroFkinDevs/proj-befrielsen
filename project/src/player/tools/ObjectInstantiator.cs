using System;
using System.IO;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class ObjectInstantiator : Node3D
    {
        [Export]
        public Node Parent;
        [Export]
        public string SpawnId = "prop";
        public MultiplayerSpawner spawner;
        public int PropsCount = 0;

        public override void _Ready()
        {
            base._Ready();
            Parent = Parent.GetParent<Node>();
            spawner = new MultiplayerSpawner();
            spawner.Name = Name + SpawnId + "Spawner";
            Parent.AddChild(spawner);
            spawner.SpawnPath = spawner.GetPath();
        }

        public void RequestInstantiate(TmpStorage tmpStorage, PackedScene scene, Node3D cbNode, StringName cbMethod)
        {
            tmpStorage.BroadcastArrayOfResources<PackedScene>(new Godot.Collections.Array<PackedScene> { scene },
            "instantiate_" + Name + SpawnId + "_" + PropsCount,
            this, MethodName.Instantiate,
                new Godot.Collections.Array<string> { cbNode.GetPath(), cbMethod, tmpStorage.GetPath() });
            PropsCount += 1;
        }
        public void Instantiate(Godot.Collections.Array<PackedScene> scenes, Godot.Collections.Array<string> recieveArgs)
        {
            var scene = scenes[0];
            spawner.AddSpawnableScene(scene.ResourcePath);

            if (Multiplayer.IsServer())
            {
                var tmpStorage = this.GetMultiplayerNode<TmpStorage>(recieveArgs[2]);
                Action<string> handleResLoaded = null;
                handleResLoaded = void (string resPath) =>
                {
                    if (scene.ResourcePath == resPath)
                    {
                        tmpStorage.OnResourceLoaded -= handleResLoaded;
                        var newNode = scene.Instantiate<Node3D>();
                        newNode.Name = "Obj_" + Path.GetFileName(scene.ResourcePath);
                        spawner.AddChild(newNode);
                    }
                };
                tmpStorage.OnResourceLoaded += handleResLoaded;
            }
            else
            {
                var cbNode = this.GetMultiplayerNode<Node3D>(recieveArgs[0]);
                var cbMethod = recieveArgs[1];
                MultiplayerSpawner.SpawnedEventHandler spawnedCB = null;
                spawnedCB = void (Node node) =>
                {
                    cbNode.Call(cbMethod, node);
                    spawner.Spawned -= spawnedCB;
                };
                spawner.Spawned += spawnedCB;
            }
        }
    }
}