using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class MultiplyPlatform : MonoBehaviour
    {
        [SerializeField] private TextMeshPro multiplierText;

        private Material _material;
        
        public float multiplier;
        public Color platformColor;

        private void Awake()
        {
            gameObject.tag = "MultiplyPlatform";
            _material = GetComponent<Renderer>().material;
        }

        public void SetMultiplier(float multiplierValue)
        {
            multiplier = multiplierValue;
            multiplierText.text = $"x{multiplier}";
            gameObject.name = new string(gameObject.name + ' ' + multiplier);
        }

        public void Coloration()
        {
            _material.color = platformColor;
        }

        public void StartAnimation()
        {
            StartCoroutine(FlashingAnim());
        }

        private IEnumerator FlashingAnim()
        {
            int i = 0;
            
            while (i <= 10)
            {
                _material.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                _material.color = platformColor;
                yield return new WaitForSeconds(0.1f);
                i++;
            }

            yield return null;
        }
    }
}