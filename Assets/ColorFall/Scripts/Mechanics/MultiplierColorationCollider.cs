using ColorFall.Mechanics;
using UnityEngine;

public class MultiplierColorationCollider : MonoBehaviour
{
    private MultiplyPlatform platform;

    void Start()
    {
        platform = GetComponentInChildren<MultiplyPlatform>();
        gameObject.name = new string(gameObject.name + ' ' + platform.multiplier);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        platform.Coloration();
    }
}
