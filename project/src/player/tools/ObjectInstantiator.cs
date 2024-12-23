using System;
using System.IO;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class ObjectInstantiator : Node3D
    {
        public LocationLoader locationLoader { get { return this.GetMultiplayerNode<LocationLoader>("/root/Main/Client/LocationLoader"); } }
        [Export]
        public string SpawnId = "prop";
        public MultiplayerSpawner spawner;
        public int PropsCount = 0;

        public override void _Ready()
        {
            base._Ready();
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

            var newNode = scene.Instantiate<Node3D>();
            newNode.Name = "Obj_" + Path.GetFileName(scene.ResourcePath);
            var parent = locationLoader.LocationInstance;
            parent.AddChild(newNode);
            newNode.Owner = parent;

            var cbNode = this.GetMultiplayerNode<Node3D>(recieveArgs[0]);
            var cbMethod = recieveArgs[1];
            cbNode.Call(cbMethod, newNode);
        }
    }
}