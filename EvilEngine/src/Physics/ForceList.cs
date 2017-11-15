using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace EvilEngine.Physics
{
    public class ForceList : IEnumerable<Vector2>
    {
        public Dictionary<ForceType, Vector2> ForceStack { get; private set; }

        public Vector2 Value { get; private set; }

        public float X => Value.X;

        public float Y => Value.Y;

        private float _mass;
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                MassInverse = 1 / value;
            }
        }

        public float MassInverse { get; private set; }

        public ForceList(float mass = 0.0f)
        {
            ForceStack = new Dictionary<ForceType, Vector2>();
            Mass = mass;
            Value = Vector2.Zero;
        }

        public void AddForce(ForceType id, Vector2 force)
        {
            if (ForceStack.ContainsKey(id)) return;
            ForceStack.Add(id, force);
            ComputeForces();
        }

        public void RemoveForce(ForceType id)
        {
            if (!ForceStack.ContainsKey(id)) return;
            ForceStack.Remove(id);
            ComputeForces();
        }

        public Vector2 GetForce(ForceType id)
        {
            if (ForceStack.TryGetValue(id, out Vector2 toReturn))
            {
                return toReturn;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        public void UpdateForce(ForceType id, Vector2 force)
        {
            if (!ForceStack.ContainsKey(id)) return;
            ForceStack[id] = force;
            ComputeForces();
        }

        public void Reset()
        {
            Value = Vector2.Zero;
            ForceStack.Clear();
        }

        private void ComputeForces()
        {
            Value = Vector2.Zero;
            foreach (Vector2 force in ForceStack.Values)
            {
                Value += force;
            }

            if (Mass > 0)
                Value *= MassInverse;
        }

        public IEnumerator<Vector2> GetEnumerator()
        {
            return ForceStack.Values.GetEnumerator() as IEnumerator<Vector2>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Copy(ForceList other)
        {
            Value = other.Value;
            Mass = other.Mass;
            ForceStack = new Dictionary<ForceType, Vector2>(other.ForceStack);
        }
        
        public void CopyIn(out ForceList other)
        {
            other = new ForceList(Mass);
            other.Value = Value;
            other.ForceStack = new Dictionary<ForceType, Vector2>(ForceStack);
        }
    }
}