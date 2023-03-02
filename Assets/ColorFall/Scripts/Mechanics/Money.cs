using UnityEngine;
using System.Collections;
using ColorFall.Game;

namespace ColorFall.Mechanics
{
    public class Money : MonoBehaviour, ICollectable
    {
        public bool IsAnimating { get; private set; }
        private float _moveSpeed;
        void Start()
        {
            transform.tag = "Money";
            IsAnimating = false;
        }

        public void Collect(Transform target)
        {
            _moveSpeed = target.GetComponent<Player>().CurrentSpeed + 5f;
            StartCoroutine(Follow(target));
        }
        
        private IEnumerator Follow(Transform target)
        {
            IsAnimating = true;
            if (Managers.Energy.IsCharged) _moveSpeed += 7f;
            while (Mathf.Abs(Vector3.Magnitude(transform.position - target.position)) > 0.5f)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, target.position, _moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            yield return null;
            Destroy(gameObject);
        }
    }
}
