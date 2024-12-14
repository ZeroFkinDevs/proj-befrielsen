using System;
using Godot;

namespace Game
{
    public partial class LivingStateResource : Resource
    {
        [Export]
        public int MaxHealth = 1;
        [Export]
        public int Health = 1;
        [Export]
        public int Effect = 1;

        public event Action<LivingStateResource, LivingStateResource> OnChange;

        public void TakeDamage(int hpDamage)
        {
            var oldState = (LivingStateResource)this.Duplicate();
            Health -= hpDamage;
            Health = Math.Max(0, Health);
            OnChange?.Invoke(oldState, this);
        }
    }
}