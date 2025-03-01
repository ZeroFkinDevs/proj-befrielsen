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
        TOUCH,
    }
    public interface IUser
    {

    }
    public interface IInteractable
    {
        public InteractionTypeEnum InteractionType { get; }
        public string InteractableDescription { get; }
        public void Interact(IUser user);
    }
    public interface IToolInteractable
    {
        public InteractionTypeEnum GetInteractionType(ITool tool, ItemStack toolStack);
        public string GetInteractableDescription(ITool tool, ItemStack toolStack);
        public void Interact(ITool tool, ItemStack toolStack);
    }
}