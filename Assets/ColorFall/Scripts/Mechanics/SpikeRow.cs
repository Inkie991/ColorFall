using System;
using System.Collections.Generic;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class SpikeRow : MonoBehaviour
    {
        private const float OffsetZ = 2f;
        private const float BaseAngle = 60f;
        
        [Range(1, 5)]
        [SerializeField]
        public int count;

        [SerializeField] 
        private bool symmetrical;

        private void Awake()
        {
            this.ConstructRow();
        }

        [ContextMenu("Construct Row")]
        public void ConstructRow()
        {
            if (count > 3 && symmetrical)
            {
                Debug.Log("Unreal to pass! Symmetrical only 1-3 sectors!");
                return;
            }

            DestroyRow();
            BuildRow();
        }

        private void BuildRow()
        {
            Vector3 spawnPos = new Vector3(0, transform.position.y, OffsetZ);
            for (int i = 0; i < count; i++)
            {
                float angle;
                if (symmetrical) angle = (360 / count) * i;
                else angle = BaseAngle * i;
                GameObject obj = Instantiate(GameObjectsLoader.GetPrefab<SpikeSector>(), spawnPos, Quaternion.identity);
                obj.transform.Rotate(Vector3.up, angle);
                obj.transform.SetParent(transform);
            }
        }

        private void DestroyRow()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                if  (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}
