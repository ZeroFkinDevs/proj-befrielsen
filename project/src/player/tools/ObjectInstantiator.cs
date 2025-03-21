using System;
using System.IO;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class ObjectInstantiator : Node3D
    {
        public WorldContainer locationLoader { get { return this.GetMultiplayerNode<WorldContainer>("/root/Main/Client/LocationLoader"); } }
        [Export]
        public string SpawnId = "prop";
        public MultiplayerSpawner spawner;
        public int ObjectsCount = 0;

        public override void _Ready()
        {
            base._Ready();
        }

        public void RequestInstantiate(TmpStorage tmpStorage, PackedScene scene, Node3D cbNode, StringName cbMethod)
        {
            tmpStorage.BroadcastArrayOfResources<PackedScene>(new Godot.Collections.Array<PackedScene> { scene },
            "instantiate_" + Name + SpawnId + "_" + ObjectsCount,
            this, MethodName.Instantiate,
                new Godot.Collections.Array<string> { cbNode.GetPath(), cbMethod, tmpStorage.GetPath() });
            ObjectsCount += 1;
        }
        public virtual void Instantiate(Godot.Collections.Array<PackedScene> scenes, Godot.Collections.Array<string> recieveArgs)
        {
            var scene = scenes[0];

            var newNode = scene.Instantiate<Node3D>();
            newNode.Name = "Obj_" + Path.GetFileName(scene.ResourcePath);
            var parent = locationLoader.LocationInstance;
            parent.AddChild(newNode);
            newNode.Owner = parent;

            var cbNode = this.GetMultiplayerNode<Node3D>(recieveArgs[0]);
            var cbMethod = recieveArgs[1];
            cbNode.Call(cbMethod, newNode);
        }
        public void RequestInstantiateProp(TmpStorage tmpStorage, ItemResource itemRes, Node3D cbNode, StringName cbMethod)
        {
            tmpStorage.BroadcastArrayOfResources<ItemResource>(new Godot.Collections.Array<ItemResource> { itemRes },
            "instantiate_prop_" + Name + SpawnId + "_" + ObjectsCount,
            this, MethodName.InstantiateProp,
                new Godot.Collections.Array<string> { cbNode.GetPath(), cbMethod, tmpStorage.GetPath() });
            ObjectsCount += 1;
        }

        public void InstantiateProp(Godot.Collections.Array<ItemResource> itemsResAr, Godot.Collections.Array<string> recieveArgs)
        {
            var itemRes = itemsResAr[0];

            var parent = locationLoader.LocationInstance;
            var (newNode, prop) = itemRes.InstantiateSimpleProp(parent);
            newNode.Name = "Obj_" + Path.GetFileName(itemRes.ResourcePath);
            newNode.Owner = parent;

            var cbNode = this.GetMultiplayerNode<Node3D>(recieveArgs[0]);
            var cbMethod = recieveArgs[1];
            cbNode.Call(cbMethod, prop);
        }
    }
}