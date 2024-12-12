using Godot;

namespace Game
{
    public partial class LivingStateManager : Node
    {
        [Export]
        public LivingStateResource livingState;

        public override void _EnterTree()
        {
            if (livingState == null) livingState = new LivingStateResource();
        }
    }
}