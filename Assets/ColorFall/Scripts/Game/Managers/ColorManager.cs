using System.Collections.Generic;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Game
{
    public static class ColorManager
    {
        private static readonly Color LightRed = new (1f, 0, 0.7f);
        private static readonly Color LightGreen = new (0.7f, 1f, 0);
        private static readonly Color LightBlue = new (0, 0.7f, 1f);
        
        public static readonly Color StandardRed = new Color(0.992f, 0.220f, 0.227f);
        private static readonly Color StandardGreen = new Color(0.098f, 0.992f, 0.404f);
        private static readonly Color StandardBlue = new Color(0.075f, 0.635f, 1f);
        private const float DefaultHueStep = 0.003f;
        private const float StandardRedHue = 0.9972f;
        private const float StandardGreenHue = 0.3917f;
        private const float StandardBlueHue = 0.5667f;

        private static readonly Dictionary<GamingColor, Material> MaterialsDictionary = new()
        {
            { GamingColor.Red, Resources.Load<Material>("Colors/Red") },
            { GamingColor.Green, Resources.Load<Material>("Colors/Green") },
            { GamingColor.Blue, Resources.Load<Material>("Colors/Blue") }
        };
        private static readonly Dictionary<GamingColor, Material> TransparentMaterialsDictionary = new()
        {
            { GamingColor.Red, Resources.Load<Material>("Colors/Red_transparent") },
            { GamingColor.Green, Resources.Load<Material>("Colors/Green_transparent") },
            { GamingColor.Blue, Resources.Load<Material>("Colors/Blue_transparent") }
        };
        private static readonly Dictionary<GamingColor, Material> SpriteMaterialsDictionary = new()
        {
            { GamingColor.Red, Resources.Load<Material>("Colors/Red_sprite") },
            { GamingColor.Green, Resources.Load<Material>("Colors/Green_sprite") },
            { GamingColor.Blue, Resources.Load<Material>("Colors/Blue_sprite") }
        };
        private static readonly Dictionary<GamingColor, Color> ColorsDictionary = new()
        {
            { GamingColor.Red, StandardRed },
            { GamingColor.Green, StandardGreen },
            { GamingColor.Blue, StandardBlue }
        };
        private static readonly Dictionary<GamingColor, GradientColorKey[]> GradientsDictionary = new()
        {
            { GamingColor.Red, new[] { new GradientColorKey(LightRed, 0.0f), new GradientColorKey(StandardRed, 1.0f) } },
            { GamingColor.Green, new[] { new GradientColorKey(LightGreen, 0.0f), new GradientColorKey(StandardGreen, 1.0f) } },
            { GamingColor.Blue, new[] { new GradientColorKey(LightBlue, 0.0f), new GradientColorKey(StandardBlue, 1.0f) } }
        };

        public static Material GetMaterial(GamingColor gamingColor, MaterialType type = MaterialType.Default)
        {
            switch (type)
            {
                case MaterialType.Transparent:
                    return TransparentMaterialsDictionary[gamingColor];
                case MaterialType.Sprite:
                    return SpriteMaterialsDictionary[gamingColor];
                case MaterialType.Default:
                default:
                    return MaterialsDictionary[gamingColor];
            }
        }

        public static Color GetColor(GamingColor gamingColor)
        {
            return ColorsDictionary[gamingColor];
        }

        public static GradientColorKey[] GetGradient(GamingColor gamingColor)
        {
            return GradientsDictionary[gamingColor];
        }

        public static Color GetNextHSVColor(Color prevColor, float hueChangeStep = 0.07f)
        {
            float H, S, V;

            Color.RGBToHSV(prevColor, out H, out S, out V);
            H += hueChangeStep;

            if (H > 1) H -= 1;

            return Color.HSVToRGB(H, S, V);
        }
        
        public static Color GetNextChargedModeColor(Color prevColor)
        {
            float H, S, V;
            
            Color.RGBToHSV(prevColor, out H, out S, out V);
            float nextH = H + DefaultHueStep;
            float rangeBorder = DefaultHueStep * 10;
            if (nextH >= StandardGreenHue - rangeBorder && nextH <= StandardGreenHue + rangeBorder)
            {
                float tempH;
                Color.RGBToHSV(StandardGreen, out tempH, out S, out V);
                if (nextH > 1f) nextH -= 1f;
                return Color.HSVToRGB(nextH, S, V);
            }
            if (nextH >= StandardBlueHue - rangeBorder && nextH <= StandardBlueHue + rangeBorder)
            {
                float tempH;
                Color.RGBToHSV(StandardBlue, out tempH, out S, out V);
                if (nextH > 1f) nextH -= 1f;
                return Color.HSVToRGB(nextH, S, V);
            }
            if ((nextH >= StandardRedHue - rangeBorder && nextH <= 1)||
                (nextH >= 0 && nextH <= StandardRedHue + rangeBorder - 1))
            {
                float tempH;
                Color.RGBToHSV(StandardRed, out tempH, out S, out V);
                if (nextH > 1f) nextH -= 1f;
                return Color.HSVToRGB(nextH, S, V);
            }

            return GetNextChargedModeColor(Color.HSVToRGB(nextH, S, V));
        }
    }
}
