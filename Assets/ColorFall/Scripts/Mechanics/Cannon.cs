using ColorFall.Game;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem explosion_VFX;
        private Animator _animator;
        private static readonly int Rotate = Animator.StringToHash("rotate");

        void Start()
        {
            _animator = GetComponent<Animator>();
            explosion_VFX.Stop();
            EventManager.AddListener<CannonEvent>(OnCannon);
            EventManager.AddListener<BoostEvent>(OnBoost);
        }
        
        void OnDestroy()
        {
            EventManager.RemoveListener<CannonEvent>(OnCannon);
            EventManager.RemoveListener<BoostEvent>(OnBoost);
        }

        void OnCannon(CannonEvent evt)
        {
            _animator.SetTrigger(Rotate);
        }

        void OnBoost(BoostEvent evt)
        {
            explosion_VFX.Play();
        }
    }
}
