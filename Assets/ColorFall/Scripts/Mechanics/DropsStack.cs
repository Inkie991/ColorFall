using System.Collections.Generic;
using System.Linq;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class DropsStack : Stack<DropsRow, Drop>
    {
        [SerializeField]
        [ContextMenuItem("Apply colors to drops", "ApplyColor")]
        private List<GamingColor> gamingColors;

        private GameObject _startOfStack;
        private GameObject _endOfStack;

        [ContextMenu("Generate stack")]
        protected override void GenerateStack() // for ContextMenu working
        {
            rows = new List<DropsRow>();
            base.GenerateStack();
            GenerateStartOfStack();
            GenerateEndOfStack();
        }

        protected override void BuildStack()
        {
            base.BuildStack();
            ApplyColor();
        }

        private void ApplyColor()
        {
            foreach (var row in rows)
            {
                List<Drop> drops = row.Items;

                for (var i = 0; i < drops.Count; i++)
                {
                    drops[i].GamingColor = gamingColors[i];
                }
            }
        }
        
        private void GenerateStartOfStack()
        {
            if (_startOfStack != null)
            {
                if  (Application.isPlaying)
                    Destroy(_startOfStack);
                else
                    DestroyImmediate(_startOfStack);
            }
            Vector3 spawnPos = new Vector3(0, 1, 0);
            _startOfStack = Instantiate(
                GameObjectsLoader.GetPrefab<PassCollider>(),
                transform
            );
            _startOfStack.transform.localPosition = spawnPos;
            _startOfStack.GetComponent<PassCollider>().isStartOfStack = true;
        }

        private void GenerateEndOfStack()
        {
            if (_endOfStack != null)
            {
                if  (Application.isPlaying)
                    Destroy(_endOfStack);
                else
                    DestroyImmediate(_endOfStack);
            }
            DropsRow lastRow = rows.Last();
            _endOfStack = Instantiate(
                GameObjectsLoader.GetPrefab<PassCollider>(),
                transform
            );
            _endOfStack.transform.localPosition = Vector3.zero + (Vector3.up * lastRow.transform.localPosition.y);
            _endOfStack.GetComponent<PassCollider>().isEndOfStack = true;
        }

        public int GetRowsCount()
        {
            return rowsCount;
        }
    }
}
