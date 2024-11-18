using Godot;
using System;

namespace Game
{
    public partial class HookesConnector : Node3D
    {
        [Export]
        public Node3D Target;
        [Export]
        public RigidBody3D Body;
        [Export]
        public bool Active = true;

        [Export]
        public float linearSpringStiffness = 400f;
        [Export]
        public float linearSpringDamping = 50f;
        [Export]
        public float maxLinearForce = 100f;
        [Export]
        public float angularSpringStiffness = 800f;
        [Export]
        public float angularSpringDamping = 30f;
        [Export]
        public float maxAngularForce = 400f;

        public override void _Ready()
        {

        }

        /// <summary>
        /// Использует формулу HookesLaw чтобы вычислить силу для перемещения в позицию и поворот Target.
        /// </summary>
        /// <param name="delta"></param>
        private void UpdateVelocity(double delta)
        {
            if (Target == null) return;
            if (Body == null) return;
            if (!IsInstanceValid(Target)) return;
            if (!IsInstanceValid(Body)) return;

            if (Body.GlobalTransform.Origin.DistanceTo(Target.GlobalTransform.Origin) > 3.0f)
            {
                Transform3D trans = Body.GlobalTransform;
                trans.Origin = Target.GlobalTransform.Origin + ((Body.GlobalTransform.Origin - Target.GlobalTransform.Origin).Normalized() * 3.0f);
                Body.GlobalTransform = trans;
            }

            Transform3D targetTransform = Target.GlobalTransform;
            Transform3D currentTransform = Body.GlobalTransform;
            Basis rotationDifference = targetTransform.Basis * currentTransform.Basis.Inverse();

            Vector3 positionDifference = targetTransform.Origin - currentTransform.Origin;
            Vector3 force = HookesLaw(positionDifference, Body.LinearVelocity, linearSpringStiffness, linearSpringDamping);
            force = force.LimitLength(maxLinearForce);
            Body.LinearVelocity += force * (float)delta;

            Vector3 torque = HookesLaw(rotationDifference.GetEuler(), Body.AngularVelocity, angularSpringStiffness, angularSpringDamping);
            torque = torque.LimitLength(maxAngularForce);

            Body.AngularVelocity += torque * (float)delta;
        }
        public override void _PhysicsProcess(double delta)
        {
            if (Active) UpdateVelocity(delta);
        }

        /// <summary>
        /// Закон Гука (Hooke's law)
        /// </summary>
        /// <param name="displacement"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="stiffness"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        private Vector3 HookesLaw(Vector3 displacement, Vector3 currentVelocity, float stiffness, float damping)
        {
            return (stiffness * displacement) - (damping * currentVelocity);
        }
    }
}
