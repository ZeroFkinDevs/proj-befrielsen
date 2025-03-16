using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Game
{
    public interface IBodyFetcher
    {
        public bool Test(object body);
        public void SetArea(Area3D area);
        public void RecieveBodyEnter(object body);
        public void RecieveBodyExit(object body);
        public void Update();
    }
    public class BodyFetcher<T> : IBodyFetcher where T : class
    {
        protected Area3D _area;
        protected T _nearestBody = null;
        public T NearestObject => _nearestBody;
        protected List<T> _bodies = new List<T>();
        public List<T> Bodies => _bodies;

        public event Action OnEmptied;
        public event Action OnFilled;
        public event Action<T> OnEntered;
        public event Action<T> OnExited;

        public void SetArea(Area3D area)
        {
            _area = area;
        }

        public T Fetch(object body)
        {
            if (body is T) return body as T;
            return null;
        }

        public void RecieveBodyEnter(object body)
        {
            if (body is T casted)
            {
                _bodies.Add(casted);
                OnEntered?.Invoke(casted);
                if (_bodies.Count == 1) OnFilled?.Invoke();
            }
        }

        public void RecieveBodyExit(object body)
        {
            if (body is T casted)
            {
                _bodies.Remove(casted);
                OnExited?.Invoke(casted);
                if (_bodies.Count == 0) OnEmptied?.Invoke();
            }
        }

        public bool Test(object body)
        {
            return body is T;
        }

        public void Update()
        {
            if (typeof(Node3D).IsAssignableFrom(typeof(T)))
            {
                Node3D nearest = null;
                float distance = 0.0f;
                foreach (var body in _bodies)
                {
                    if (body is Node3D node3D)
                    {
                        if (nearest == null)
                        {
                            nearest = node3D;
                        }
                        else if (node3D.GlobalPosition.DistanceTo(_area.GlobalPosition) < distance)
                        {
                            nearest = node3D;
                        }
                        distance = nearest.GlobalPosition.DistanceTo(_area.GlobalPosition);
                    }
                }
                _nearestBody = nearest as T;
            }
        }
    }

    public partial class BodyDetectionArea : Area3D
    {
        [Export]
        public bool Enabled = true;

        private List<IBodyFetcher> fetchers = new List<IBodyFetcher>();

        public override void _EnterTree()
        {
            base._EnterTree();
            BodyEntered += OnSmthEntered;
            AreaEntered += OnSmthEntered;

            BodyExited += OnSmthExited;
            AreaExited += OnSmthExited;
        }

        public void OnSmthEntered(Node3D body)
        {
            foreach (var fetcher in fetchers)
            {
                fetcher.RecieveBodyEnter(body);
            }
        }
        public void OnSmthExited(Node3D body)
        {
            foreach (var fetcher in fetchers)
            {
                fetcher.RecieveBodyExit(body);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
            foreach (var fetcher in fetchers)
            {
                fetcher.Update();
            }
        }

        public void AddFetcher(IBodyFetcher fetcher)
        {
            fetcher.SetArea(this);
            fetchers.Add(fetcher);
        }
    }
}