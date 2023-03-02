using UnityEngine;

namespace ColorFall.Mechanics
{
    public class MoneyRow : Row<Money>
    {
        [ContextMenu("Construct Row")]
        public new void ConstructRow()
        {
            base.ConstructRow();
        }
    }
}
