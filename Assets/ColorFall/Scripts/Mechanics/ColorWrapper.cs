using System.Collections.Generic;
using System.Linq;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class ColorWrapper : MonoBehaviour
    { 
        [SerializeField]
        private List<GamingColor> gamingColors;
        private List<ColorChanger> _changerSectors;

        private void Start()
        {
            ApplyColor();
        }

        [ContextMenu("Apply color")]
        private void ApplyColor()
        {
            _changerSectors = GetComponentsInChildren<ColorChanger>().ToList();

            if (gamingColors.Count > _changerSectors.Count)
            {
                gamingColors.RemoveRange(_changerSectors.Count, gamingColors.Count);
            }

            for (int i = 0; i < _changerSectors.Count; i++)
            {
                _changerSectors[i].GamingColor = gamingColors[i];
            }
        }
    }
}
