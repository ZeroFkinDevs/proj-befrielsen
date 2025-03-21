using System;
using System.IO;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class ObjectInstantiator : Node3D
    {
        [Export]
        public string SpawnId = "prop";
        public int ObjectsCount = 0;

        public override void _Ready()
        {
            base._Ready();
        }

        public void RequestInstantiate(PackedScene scene, Node3D cbNode, StringName cbMethod)
        {
            this.GetResourcesBroadcaster().BroadcastArrayOfResources<PackedScene>(new Godot.Collections.Array<PackedScene> { scene },
            this, MethodName.Instantiate,
                new Godot.Collections.Array<string> { cbNode.GetPath(), cbMethod });
        }
        public virtual void Instantiate(Godot.Collections.Array<PackedScene> scenes, Godot.Collections.Array<string> recieveArgs)
        {
            var scene = scenes[0];

            var newNode = scene.Instantiate<Node3D>();
            newNode.Name = "Obj_" + Path.GetFileName(scene.ResourcePath);
            var parent = this.GetWorldContainer().LocationInstance;
            parent.AddChild(newNode);
            newNode.Owner = parent;

            var cbNode = this.GetMultiplayerNode<Node3D>(recieveArgs[0]);
            var cbMethod = recieveArgs[1];
            cbNode.Call(cbMethod, newNode);
        }
        public void RequestInstantiateProp(ItemResource itemRes, Node3D cbNode, StringName cbMethod)
        {
            this.GetResourcesBroadcaster().BroadcastArrayOfResources<ItemResource>(new Godot.Collections.Array<ItemResource> { itemRes },
            this, MethodName.InstantiateProp,
                new Godot.Collections.Array<string> { cbNode.GetPath(), cbMethod });
        }

        public void InstantiateProp(Godot.Collections.Array<ItemResource> itemsResAr, Godot.Collections.Array<string> recieveArgs)
        {
            var itemRes = itemsResAr[0];

            var parent = this.GetWorldContainer().LocationInstance;
            var (newNode, prop) = itemRes.InstantiateSimpleProp(parent);
            newNode.Name = "Obj_" + Path.GetFileName(itemRes.ResourcePath);
            newNode.Owner = parent;

            var cbNode = this.GetMultiplayerNode<Node3D>(recieveArgs[0]);
            var cbMethod = recieveArgs[1];
            cbNode.Call(cbMethod, prop);
        }
    }
}