using System.Collections;
using ColorFall.Core;
using ColorFall.Game;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public abstract class ColorableObject : MonoBehaviour
    {
        private Coroutine _rgbAnimationCoroutine;
        protected bool isRgbAnimationEnabled = false;
        
        [SerializeField]
        [ContextMenuItem("Apply color", "ChangeMaterial")]
        protected GamingColor gamingColor;
        
        public GamingColor GamingColor
        {
            get => gamingColor;
            set
            {
                gamingColor = value;
                ChangeMaterial();
            }
        }

        protected void Awake()
        {
            GetComponent<Renderer>().material = GetComponent<Renderer>().material;
            ChangeMaterial();

            if (isRgbAnimationEnabled)
            {
                EventManager.AddListener<ChargedModeOnEvent>(RgbAnimationOn);
                EventManager.AddListener<ChargedModeOffEvent>(RgbAnimationOff);
            }
        }

        protected void OnDestroy()
        {
            if (isRgbAnimationEnabled)
            {
                EventManager.RemoveListener<ChargedModeOnEvent>(RgbAnimationOn);
                EventManager.RemoveListener<ChargedModeOffEvent>(RgbAnimationOff);
            }
        }

        protected virtual void ChangeMaterial()
        {
            GetComponent<Renderer>().material = ColorManager.GetMaterial(gamingColor);
        }

        private void RgbAnimationOn(ChargedModeOnEvent evt)
        {
            _rgbAnimationCoroutine = StartCoroutine(RgbAnimation());
        }

        private void RgbAnimationOff(ChargedModeOffEvent evt)
        {
            if (_rgbAnimationCoroutine != null)
            {
                StopCoroutine(_rgbAnimationCoroutine);
                _rgbAnimationCoroutine = null;
            }
            
            ChangeMaterial();
        }

        private IEnumerator RgbAnimation()
        {
            GetComponent<Renderer>().material.color = ColorManager.StandardRed;

            while (true)
            {
                GetComponent<Renderer>().material.color = ColorManager.GetNextChargedModeColor(GetComponent<Renderer>().material.color);
                yield return new WaitForSeconds(0.001f);
            }
        }
    }
}
