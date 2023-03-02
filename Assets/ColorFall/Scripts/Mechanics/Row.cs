using System.Collections.Generic;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class Row<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Range(1, 20)]
        [SerializeField]
        public int count;
        public List<T> Items { get; } = new();

        public void ConstructRow()
        {
            DestroyRow();
            BuildRow();
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
            
            Items.Clear();
        }

        private void BuildRow()
        {
            Vector3 spawnPos = new Vector3(0, transform.position.y, 0);
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(GameObjectsLoader.GetPrefab<T>(), transform);
                obj.transform.position = spawnPos;
                transform.Rotate(Vector3.up, 360 / count);
                Items.Add(obj.GetComponent<T>());
            }
        }
    }
}
