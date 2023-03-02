using ColorFall.Game;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class MultiplyPlatformsLine : MonoBehaviour
    {
        [SerializeField] private GameObject multiplyPlatform;

        private void Start()
        {
            GenerateMultiplyPlatforms();
        }
        
        private void GenerateMultiplyPlatforms()
        {
            Color nextPlatformColor = Color.HSVToRGB(0, 1, 1);;
            Vector3 nextPlatformPos = transform.position;
            
            for (decimal multiplier = 1; multiplier <= SavesManager.MaxMultiplier; multiplier += (decimal)0.1)
            {
                var position = nextPlatformPos;
                nextPlatformPos.x += 10;
                var obj = Instantiate<GameObject>(multiplyPlatform, position, transform.rotation, transform);
                var multiPlatform = obj.GetComponentInChildren<MultiplyPlatform>();
                multiPlatform.SetMultiplier((float)multiplier);
                multiPlatform.platformColor = nextPlatformColor;
                nextPlatformColor = ColorManager.GetNextHSVColor(multiPlatform.platformColor);
            }
        }
    }
}