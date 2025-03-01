using System;
using Game.Utils;
using Godot;

namespace Game
{
    public interface ITeleportablePoint
    {
        void RecievePointTeleportation(Func<Vector3, Vector3> teleportPoint);
    }
    public interface ITeleportableTransform
    {
        void RecieveTransformTeleportation(Func<Transform3D, Transform3D> teleportTransform);
    }
}