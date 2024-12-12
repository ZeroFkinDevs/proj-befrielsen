using Godot;

namespace Game
{
    public interface ILiving
    {
        LivingStateManager livingStateManager { get; }
    }
}