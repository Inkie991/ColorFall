using System.Collections;
using UnityEngine;

namespace ColorFall.UI
{
    public abstract class Popup : MonoBehaviour
    {
        protected RectTransform objRect;
        protected RectTransform targerRect;

        protected virtual void Start()
        {
            objRect = GetComponent<RectTransform>();
            transform.SetParent(targerRect);
            objRect.anchorMax = targerRect.anchorMax;
            objRect.anchorMin = targerRect.anchorMin;
            objRect.pivot = targerRect.pivot;
        }

        protected abstract IEnumerator Animation();

        protected void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}