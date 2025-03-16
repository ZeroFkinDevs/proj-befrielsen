using Godot;
using System;

namespace Game
{
    public partial class AnimationControllerUser : Node3D, IAnimationControllerUser
    {
        [Export]
        public AnimationController _animController;

        public AnimationController animationController => _animController;

    }
}