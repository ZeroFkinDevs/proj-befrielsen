using Godot;
using System;

namespace Game
{
    public partial class PlayerDetectArea : Area3D
    {
        public int PlayersCount = 0;

        public event Action<Player> PlayerEntered;
        public event Action<Player> PlayerExited;

        public override void _Ready()
        {
            BodyEntered += _PlayerEntered;
            BodyExited += _PlayerExited;

            CountPlayers();
        }

        public void CountPlayers()
        {
            foreach (var node in GetOverlappingBodies())
            {
                if (node is Player player)
                {
                    PlayersCount += 1;
                }
            }
        }

        void _PlayerEntered(Node3D node)
        {
            if (node is Player player)
            {
                PlayersCount += 1;
                PlayerEntered?.Invoke(player);
            }
        }
        void _PlayerExited(Node3D node)
        {
            if (node is Player player)
            {
                PlayersCount -= 1;
                PlayerExited?.Invoke(player);
            }
        }
    }

}