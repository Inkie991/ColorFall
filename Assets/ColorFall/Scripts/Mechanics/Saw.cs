using UnityEngine;

namespace ColorFall.Mechanics
{
    public class Saw: Unbreakable
    {
        [SerializeField] private bool isReversed;

        private readonly float _speed = 300f;
        private void Update()
        {
            float y = Time.deltaTime * _speed * (isReversed ? -1 : 1);
            transform.Rotate(new Vector3(0, y, 0));
        }
    }
}