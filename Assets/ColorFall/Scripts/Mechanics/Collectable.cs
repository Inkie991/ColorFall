using UnityEngine;

namespace ColorFall.Mechanics
{
    public interface ICollectable
    {
        public bool IsAnimating { get; }
        public void Collect(Transform target);
    }
}
