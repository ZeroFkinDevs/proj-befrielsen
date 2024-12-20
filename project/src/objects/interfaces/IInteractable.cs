using Godot;

namespace Game
{
    public enum InteractionTypeEnum
    {
        NONE,
        GRAB,
        PICKUP,
        INVENTORY_DRAG,
        APPLY,
    }
    public interface IInteractable
    {
        public InteractionTypeEnum InteractionType { get; }
        public string InteractableDescription { get; }
        public void Interact(IUser user);
    }
    public interface IUser
    {

    }
}