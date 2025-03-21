using System;
using System.Data.SqlTypes;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class FingersManager : Node3D
    {
        [Export]
        public Player player;
        [Export]
        public PackedScene deadFingerScene;

        public Array<LivingStateResource> fingersAlive = null;

        public override void _Ready()
        {
            player.livingStateManager.OnLivingStateChange += OnStateChange;
            fingersAlive = player.livingStateManager.livingStates.Duplicate();
        }
        public override void _ExitTree()
        {
            player.livingStateManager.OnLivingStateChange -= OnStateChange;
        }

        public void OnStateChange(LivingStateResource oldState, LivingStateResource stateRes)
        {
            UpdateAliveFingers();
        }

        public void DropFinger(int idx)
        {
            fingersAlive[idx] = null;
            UpdateFinger(idx);
            if (player.Controllable)
            {
                player.objectInstantiator.RequestInstantiate(deadFingerScene, this, MethodName.DropFingerProp);
            }
        }
        public void DropFingerProp(Node node)
        {
            Prop prop = null;
            var targetPos = GlobalPosition;

            foreach (var child in node.GetChildren())
            {
                if (child is Prop childprop)
                {
                    prop = childprop;
                    break;
                }
            }
            if (prop != null)
            {
                prop.GlobalPosition = targetPos;
                if (player.Controllable)
                {
                    prop.RequestImpulse(-player.GlobalTransform.Basis.Z, prop.GlobalTransform);
                }
            }
        }
        public void HealFinger(int idx)
        {
            fingersAlive[idx] = player.livingStateManager.livingStates[idx];
            UpdateFinger(idx);
        }

        public void UpdateFinger(int idx)
        {
            var stateRes = fingersAlive[idx];
            int boneId = player.model.skeleton3D.FindBone("finger_" + idx);
            if (stateRes == null)
            {
                player.model.skeleton3D.SetBonePoseScale(boneId, Vector3.One * -0.01f);
            }
            else
            {
                player.model.skeleton3D.SetBonePoseScale(boneId, Vector3.One);
            }
        }

        public void UpdateAliveFingers()
        {
            for (int i = 0; i < fingersAlive.Count; i++)
            {
                if (fingersAlive[i] == null && player.livingStateManager.livingStates[i].Health > 0)
                {
                    HealFinger(i);
                }
                if (fingersAlive[i] != null && player.livingStateManager.livingStates[i].Health == 0)
                {
                    DropFinger(i);
                }

                if (fingersAlive[i] != null)
                {
                    if (fingersAlive[i] != player.livingStateManager.livingStates[i])
                    {
                        fingersAlive[i] = player.livingStateManager.livingStates[i];
                        UpdateFinger(i);
                    }
                }
            }
        }
    }
}