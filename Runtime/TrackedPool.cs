using System.Collections.Generic;

namespace _Source.Gameplay.Pools.Common
{
    public class TrackedPool<TKey, TValue>
        where TValue : class, IKeyed<TKey>
    {
        private readonly Pool<TValue>             _inner;
        private readonly Dictionary<TKey, TValue> _spawned = new Dictionary<TKey, TValue>();

        public int SpawnedCount => _spawned.Count;

        public TrackedPool(Pool<TValue> inner)
        {
            _inner = inner;
        }

        public TValue RentAndRegister()
        {
            var value = _inner.Rent();
            _spawned[value.Key] = value;
            return value;
        }

        public void Return(TValue value)
        {
            if (value == null)
            {
                return;
            }

            _spawned.Remove(value.Key);
            _inner.Return(value);
        }

        public bool TryGetSpawned(TKey key, out TValue projectile) =>
            _spawned.TryGetValue(key, out projectile);
    }
}