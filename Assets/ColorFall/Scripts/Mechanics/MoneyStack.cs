using System.Collections.Generic;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class MoneyStack : Stack<MoneyRow, Money>
    {
        [ContextMenu("Generate stack")]
        protected override void GenerateStack() // for ContextMenu working
        {
            rows = new List<MoneyRow>();
            base.GenerateStack();
        }
    }
}
