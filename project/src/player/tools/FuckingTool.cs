using System;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class FuckingTool : Node3D, ITool
    {
        private ToolsManager _toolsManager;
        public ToolsManager toolsManager { get => _toolsManager; set { _toolsManager = value; } }

        [Export]
        public DamageArea damageArea;

        public Timer timer;

        public bool GetHandsLockToCamera()
        {
            return true;
        }

        public override void _Ready()
        {
            toolsManager.player.model.SetHandsAction(CharacterModel.HandsActionType.STICK_ATTACK);
            toolsManager.player.model.ObserveLockToCamera += GetHandsLockToCamera;

            damageArea.IgnoreList.Add(toolsManager.player);

            // toolsManager.player.model.animWithEvents.OnEventInvoked += OnAnimationEvent;
        }

        public override void _ExitTree()
        {
            toolsManager.player.model.SetHandsAction(CharacterModel.HandsActionType.NONE);
            toolsManager.player.model.ObserveLockToCamera -= GetHandsLockToCamera;

            // toolsManager.player.model.animWithEvents.OnEventInvoked -= OnAnimationEvent;
        }

        public override void _Process(double delta)
        {
            if (toolsManager.player.Controllable && toolsManager.player.ControlGroup == Player.ControlGroupEnum.WORLD)
            {
                if (Input.IsActionJustPressed("fire"))
                {
                    RequestAttack();
                    timer = new Timer();
                    AddChild(timer);
                    timer.Start(0.417f);
                    timer.Connect(Timer.SignalName.Timeout, Callable.From(MakeDamage));
                }
            }
        }

        public void RequestAttack()
        {
            RpcId(1, MethodName.ServerAttack);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerAttack()
        {
            Rpc(MethodName.RecieveAttack);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveAttack()
        {
            toolsManager.player.model.TriggerOneShotAction();
        }

        public void MakeDamage()
        {
            if (timer != null)
            {
                timer.QueueFree();
                timer = null;
            }

            damageArea.Damage();
        }

        public void OnAnimationEvent(string eventKey)
        {
            if (eventKey == "attack")
            {
                damageArea.Damage();
            }
        }
    }
}