using Godot;
using System;

namespace Game
{
    public partial class NpcInteractReroute : Area3D, ILiving, IInteractable, IToolInteractable
    {
        [Export]
        public NpcCharacterUnit npc;

        public LivingStateManager livingStateManager => npc.livingStateManager;

        public InteractionTypeEnum InteractionType => npc.InteractionType;

        public string InteractableDescription => npc.InteractableDescription;

        public string GetInteractableDescription(ITool tool, ItemStack toolStack)
        {
            return npc.GetInteractableDescription(tool, toolStack);
        }

        public InteractionTypeEnum GetInteractionType(ITool tool, ItemStack toolStack)
        {
            return npc.GetInteractionType(tool, toolStack);
        }

        public void Interact(ITool tool, ItemStack toolStack)
        {
            npc.Interact(tool, toolStack);
        }

        public void Interact(IUser user)
        {
            npc.Interact(user);
        }
    }
}
