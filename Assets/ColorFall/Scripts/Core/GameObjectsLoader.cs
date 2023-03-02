using ColorFall.Mechanics;
using UnityEngine;

namespace ColorFall.Core
{
    public static class GameObjectsLoader
    {
        private static GameObject _passColliderPrefab;
        private static GameObject PassColliderPrefab
        {
            get
            {
                if (_passColliderPrefab == null)
                    _passColliderPrefab = Resources.Load<GameObject>("Prefabs/pass_collider");
                return _passColliderPrefab;
            }
        }

        private static GameObject _coinPrefab;
        private static GameObject CoinPrefab
        {
            get
            {
                if (_coinPrefab == null)
                    _coinPrefab = Resources.Load<GameObject>("Prefabs/coin");
                return _coinPrefab;
            }
        }

        private static GameObject _dropPrefab;
        private static GameObject DropPrefab
        {
            get
            {
                if (_dropPrefab == null)
                    _dropPrefab = Resources.Load<GameObject>("Prefabs/drop");
                return _dropPrefab;
            }
        }

        private static GameObject _smudgePrefab;
        private static GameObject SmudgePrefab
        {
            get
            {
                if (_smudgePrefab == null)
                    _smudgePrefab = Resources.Load<GameObject>("Prefabs/smudge");
                return _smudgePrefab;
            }
        }
        
        private static GameObject _spikeSectorPrefab;
        private static GameObject SpikeSectorPrefab
        {
            get
            {
                if (_spikeSectorPrefab == null)
                    _spikeSectorPrefab = Resources.Load<GameObject>("Prefabs/spike");
                return _spikeSectorPrefab;
            }
        }
        
        private static GameObject _moneyRowPrefab;
        private static GameObject MoneyRowPrefab
        {
            get
            {
                if (_moneyRowPrefab == null)
                    _moneyRowPrefab = Resources.Load<GameObject>("Prefabs/money_row");
                return _moneyRowPrefab;
            }
        }
        
        private static GameObject _dropsRowPrefab;
        private static GameObject DropsRowPrefab
        {
            get
            {
                if (_dropsRowPrefab == null)
                    _dropsRowPrefab = Resources.Load<GameObject>("Prefabs/drops_row");
                return _dropsRowPrefab;
            }
        }
        
        public static GameObject GetPrefab<T>()
        {
            if (typeof(T) == typeof(Money))
                return CoinPrefab;
            if (typeof(T) == typeof(Drop))
                return DropPrefab;
            if (typeof(T) == typeof(Smudge))
                return SmudgePrefab;
            if (typeof(T) == typeof(SpikeSector))
                return SpikeSectorPrefab;
            if (typeof(T) == typeof(PassCollider))
                return PassColliderPrefab;
            if (typeof(T) == typeof(MoneyRow))
                return MoneyRowPrefab;
            if (typeof(T) == typeof(DropsRow))
                return DropsRowPrefab;

            return null;
        }
    }
}
