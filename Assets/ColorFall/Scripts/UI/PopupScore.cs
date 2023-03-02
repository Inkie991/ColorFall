using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace ColorFall.UI
{
    public class PopupScore : Popup
    {
        private TextMeshProUGUI _text;

        protected override void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _text.fontSize = 40f;
            targerRect = GameObject.FindGameObjectWithTag("ScoreLabel").GetComponent<RectTransform>();
            base.Start();
            objRect.sizeDelta = targerRect.sizeDelta;
            StartCoroutine(Animation());
        }

        protected override IEnumerator Animation()
        {
            float duration = 0.15f;
            objRect.DOAnchorPosY(0f - targerRect.sizeDelta.y, duration); 
            DOTween.To(() => _text.fontSize, value => _text.fontSize = value, 120f, duration);
            yield return new WaitForSeconds(duration);
            DestroySelf();
        }

    }
}

