using System.Collections.Generic;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Mechanics
{
    [SelectionBase]
    public abstract class Stack<TRow, TValue> : MonoBehaviour where TRow : Row<TValue> where TValue : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 50)]
        protected int rowsCount;

        [SerializeField]
        [Range(1, 10)]
        private int itemsInRowCount;

        [SerializeField]
        [Range(-180f, 180f)]
        protected float angle;
        
        [SerializeField]
        [Range(1, 8f)]
        protected float offsetY;

        [SerializeField] protected GenerationMode mode;

        protected IList<TRow> rows;

        protected virtual void GenerateStack()
        {
            DestroyStack();
            BuildStack();
        }

        protected virtual void Awake()
        {
            GenerateStack();
        }

        protected void OnDestroy()
        {
            DestroyStack();
        }

        protected virtual void BuildStack()
        {
            rows.Clear();
            var initialAngle = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            for (var i = 0; i < rowsCount; i++)
            {
                var posY = transform.position.y - (offsetY * i);
                transform.Rotate(Vector3.up * angle);
                var childTransform = Utils.SetCoordinate(transform.position, posY);
                var rowObj = Instantiate(GameObjectsLoader.GetPrefab<TRow>(), childTransform, transform.rotation);
                var rowComponent = rowObj.GetComponent<TRow>();
                rowComponent.count = itemsInRowCount;
                rowComponent.ConstructRow();
                rowObj.transform.parent = transform;
                rows.Add(rowComponent);
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, initialAngle, 0));
        }

        private void DestroyStack()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                if  (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private float GetRotation(int index)
        {
            switch (mode)
            {
                case GenerationMode.Puzzle:
                    return angle * 2 * (index % 2 == 0 ? 1 : -1);
                case GenerationMode.Linear:
                default:
                    return (angle * index);
            }
        }
    }
}
