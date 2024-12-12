using Godot;

namespace Game
{
    public partial class LivingStateResource : Resource
    {
        [Export]
        public int MaxHealth = 10;
        [Export]
        public int Health = 10;
    }
}