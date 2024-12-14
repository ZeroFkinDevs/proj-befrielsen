using Godot;
using Godot.Collections;

namespace Game
{
    public partial class DamageArea : Area3D
    {
        public Array<Node3D> IgnoreList = new Array<Node3D>();

        public void Damage()
        {
            if (!Multiplayer.IsServer()) return;

            foreach (var body in GetOverlappingBodies())
            {
                if (IgnoreList.Contains(body)) continue;

                if (body is ILiving living)
                {
                    living.livingStateManager.TakeDamage(1);
                }
            }
        }
    }
}