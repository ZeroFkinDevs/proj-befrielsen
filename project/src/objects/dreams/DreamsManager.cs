using System.IO;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class DreamsManager : ObjectInstantiator
    {
        private float DreamHeight = 50.0f;
        private float DreamDistance = 50.0f;

        public override void _Ready()
        {
            base._Ready();
            SetEditableInstance(this, true);
            Owner = GetParent();
        }

        public void ServerGetOrCreateDream(PackedScene scene, Node3D cbNode, StringName cbMethod)
        {
            this.RequestInstantiate(scene, cbNode, cbMethod);
        }
        public DreamContainer FindDreamContainerByCode(string dreamCode)
        {
            foreach (var child in GetChildren())
            {
                if (child is DreamContainer dreamContainer)
                {
                    if (dreamContainer.GetFreeAdapters().Count == 0) continue;
                    if (dreamContainer.DreamCode == dreamCode) return dreamContainer;
                }
            }
            return null;
        }

        // кастомизируем Instantiate от ObjectInstantiator
        public override void Instantiate(Array<PackedScene> scenes, Array<string> recieveArgs)
        {
            var scene = scenes[0];

            var cbNode = this.GetMultiplayerNode<Node3D>(recieveArgs[0]);
            var cbMethod = recieveArgs[1];

            var newNode = scene.Instantiate<DreamContainer>();
            var foundContainer = FindDreamContainerByCode(newNode.DreamCode);
            if (foundContainer != null)
            {
                newNode.QueueFree();
                cbNode.Call(cbMethod, foundContainer);
                return;
            }

            newNode.Name = newNode.DreamCode + "_" + Path.GetFileName(scene.ResourcePath);
            var parent = this;

            parent.AddChild(newNode);
            newNode.Owner = parent.GetParent();
            newNode.SetEditableInstance(newNode, true);
            newNode.GlobalPosition = Vector3.Down * DreamHeight;

            cbNode.Call(cbMethod, newNode);

            DreamHeight += DreamDistance;
        }
    }
}