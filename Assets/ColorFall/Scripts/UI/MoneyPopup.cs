using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ColorFall.UI
{
    public class MoneyPopup : Popup
    {
        protected override void Start()
        {
            targerRect = GameObject.FindGameObjectWithTag("MoneyLabel").GetComponent<RectTransform>();
            base.Start();
            StartCoroutine(Animation());
        }

        protected override IEnumerator Animation()
        {
            float duration = 0.5f;
            Vector2 endPos = Vector2.zero;
            endPos.x += targerRect.sizeDelta.x / 2;
            objRect.DOAnchorPos(endPos, duration);
            yield return new WaitForSeconds(duration);
            DestroySelf();
        }
    }
}

