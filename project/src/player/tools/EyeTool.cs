using System;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class EyeTool : Node3D, ITool
    {
        private ToolsManager _toolsManager;
        public ToolsManager toolsManager { get => _toolsManager; set { _toolsManager = value; } }

        [Export]
        public Node3D model;

        public bool GetHandsLockToCamera()
        {
            return true;
        }

        public override void _Ready()
        {
            toolsManager.player.model.SetHandsAction(CharacterModel.HandsActionType.STICK_ATTACK);
            toolsManager.player.model.ObserveLockToCamera += GetHandsLockToCamera;
        }

        public override void _ExitTree()
        {
            toolsManager.player.model.SetHandsAction(CharacterModel.HandsActionType.NONE);
            toolsManager.player.model.ObserveLockToCamera -= GetHandsLockToCamera;
        }

        public override void _Process(double delta)
        {

        }
    }
}