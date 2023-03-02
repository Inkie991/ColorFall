using UnityEngine;

namespace ColorFall.Mechanics
{
    public class SawWrapper: MonoBehaviour
    {
        [SerializeField] private bool isReversed;
        [SerializeField] private float speed = 10f;
        private void Update()
        {
            float y = Time.deltaTime * speed * (isReversed ? -1 : 1);
            transform.Rotate(new Vector3(0, y, 0));
        }
    }
}