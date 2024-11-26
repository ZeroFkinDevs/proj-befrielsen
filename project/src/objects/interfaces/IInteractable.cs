using Godot;

namespace Game
{
    public enum InteractionTypeEnum
    {
        NONE,
        GRAB,
        PICKUP
    }
    public interface IInteractable
    {
        public InteractionTypeEnum InteractionType { get; }
        public void Interact(IUser user);
    }
    public interface IUser
    {

    }
}