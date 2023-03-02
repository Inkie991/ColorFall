using ColorFall.Core;
using ColorFall.Game;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace ColorFall.Mechanics
{
    public class Smudge : ColorableObject
    {
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private ParticleSystem splashVfx;
        private SpriteRenderer _spriteRenderer;

        public float Size { get; set; } = 1;
        public new GamingColor GamingColor
        {
            set
            {
                gamingColor = value;
                ChangeMaterial();
            }
        }

        private new void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = GetRandomSprite();
            Invoke("DestroySelf", lifeTime + 0.5f);
            Invoke("FadeOut", lifeTime - 1f);
        }

        private void Start()
        {
            transform.localScale *= Size;
            splashVfx.transform.localScale *= Size;
            PlaySplash();
        }

        Sprite GetRandomSprite()
        {
            int randomNumber = Random.Range(1, 11);
            return Resources.Load<Sprite>($"Smudges/{randomNumber}");
        }

        void DestroySelf()
        {
            Destroy(gameObject);
        }

        void FadeOut()
        {
            DOTween.To(() => _spriteRenderer.color.a, value =>
            {
                var color = _spriteRenderer.color;
                color.a = value;
                _spriteRenderer.color = color;
            }, 0f, 1f);
        }

        void PlaySplash()
        {
            ParticleSystem.MainModule splashVfxMain = splashVfx.main;
            splashVfxMain.startColor = ColorManager.GetColor(gamingColor);
            splashVfx.Play();
        }

        public void CancelRemoval()
        {
            CancelInvoke("DestroySelf");
            CancelInvoke("FadeOut");
        }

        protected override void ChangeMaterial()
        {
            _spriteRenderer.color = ColorManager.GetColor(gamingColor);
        }
    }
}
