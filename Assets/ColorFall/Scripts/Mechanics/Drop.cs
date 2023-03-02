using System.Collections;
using ColorFall.Game;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class Drop : ColorableObject, ICollectable
    {
        public bool IsAnimating { get; private set; }
        public bool IsAnimatedWhileCharge { get; private set; }
        
        private bool _isUpper = false;
        private bool _isBigger = false;
        private float _moveSpeed;
        
        private new void Awake()
        {
            isRgbAnimationEnabled = true;
            IsAnimating = false;
            IsAnimatedWhileCharge = false;
            base.Awake();
        }

        private void Update()
        {
            AnimateDrop();
        }

        private void AnimateDrop()
        {
            transform.Rotate(Vector3.up, 100f * Time.deltaTime);
        
            if (transform.localScale.x >= 1.1f)
                _isBigger = false;
            else if (transform.localScale.x <= 0.8f)
                _isBigger = true;
        
            if (transform.localPosition.y >= 0.5f)
                _isUpper = false;
            else if (transform.localPosition.y <= -0.5f)
                _isUpper = true;
            
            transform.localScale += Vector3.one * (Time.deltaTime * (_isBigger ? 0.5f : -0.5f));
            transform.localPosition += Vector3.up * (Time.deltaTime * (_isUpper ? 0.5f : -0.5f));
        }

        public void Collect(Transform target)
        {
            if (Managers.Energy.IsCharged) IsAnimatedWhileCharge = true;
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
