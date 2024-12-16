using System;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class LivingStateManager : Node3D
    {
        [Export]
        public int MaxStatesCount;
        [Export]
        public TmpStorage tmpStorage;
        [Export]
        public Array<LivingStateResource> livingStates;

        public event Action<LivingStateResource, LivingStateResource> OnLivingStateChange;

        public void ClearStates()
        {
            if (livingStates != null)
            {
                foreach (var state in livingStates)
                {
                    state.OnChange -= ProcessLivingStateChange;
                }
            }
            livingStates = new Array<LivingStateResource>();
        }
        public void FillLivingStates()
        {
            ClearStates();
            for (int i = 0; i < MaxStatesCount; i++)
            {
                var stateRes = new LivingStateResource();
                stateRes.OnChange += ProcessLivingStateChange;
                livingStates.Add(stateRes);
            }
        }

        public void ProcessLivingStateChange(LivingStateResource oldState, LivingStateResource stateRes)
        {
            if (livingStates.Contains(stateRes))
            {
                var idx = livingStates.IndexOf(stateRes);
                tmpStorage.BroadcastArrayOfResources(
                    new Array<LivingStateResource> { stateRes },
                    "livingStateChange", this,
                    MethodName.RecieveLivingStateChange,
                    new Array<string> { idx.ToString() });
            }
        }

        public void RecieveLivingStateChange(Array<LivingStateResource> singleStateRes, Array<string> args = null)
        {
            var stateRes = singleStateRes[0];
            var idx = int.Parse(args[0]);
            var oldState = (LivingStateResource)livingStates[idx].Duplicate();
            livingStates[idx].OnChange -= ProcessLivingStateChange;
            livingStates[idx] = stateRes;
            livingStates[idx].OnChange += ProcessLivingStateChange;
            OnLivingStateChange?.Invoke(oldState, stateRes);
        }

        public override void _EnterTree()
        {
            FillLivingStates();
        }
        public override void _ExitTree()
        {
            ClearStates();
        }


        public void TakeDamage(int hpDamage)
        {
            var aliveStates = new Array<LivingStateResource>();
            foreach (var state in livingStates)
            {
                if (state.Health > 0) aliveStates.Add(state);
            }
            if (aliveStates.Count > 0)
            {
                aliveStates.PickRandom().TakeDamage(hpDamage);
            }
        }
        public bool Heal(int hp)
        {
            foreach (var state in livingStates)
            {
                if (state.Health < state.MaxHealth)
                {
                    state.Heal(hp);
                    return true;
                }
            }
            return false;
        }
    }
}