using ColorFall.Core;
using ColorFall.Game;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class ColorChanger : ColorableObject
    {
        [SerializeField] private ParticleSystem particles;

        protected override void ChangeMaterial()
        {
            GetComponent<Renderer>().material = ColorManager.GetMaterial(gamingColor, MaterialType.Transparent);
            var settings = particles.main;
            var gradient = new Gradient();
            gradient.SetKeys(
                ColorManager.GetGradient(gamingColor),
                new[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(0.5f, 1.0f) }
            );
            var minMaxGradient = new ParticleSystem.MinMaxGradient(gradient)
            {
                mode = ParticleSystemGradientMode.RandomColor
            };
            settings.startColor = minMaxGradient;
        }
    }
}