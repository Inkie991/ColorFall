using System;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class DropsRow : Row<Drop>
    {
        [ContextMenu("Construct Row")]
        public new void ConstructRow()
        {
            base.ConstructRow();
        }
    }
}