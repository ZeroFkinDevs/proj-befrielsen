using System;
using System.IO;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class SceneUnwrapper : Node3D
    {
        public override void _Ready()
        {
            GetParent().SetEditableInstance(this, true);
        }
    }
}