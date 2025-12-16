using System;

namespace _Source.Gameplay.Pools.Common
{
    public enum OverflowBehavior
    {
        Expand,  // Array.Resize (multiply by 2)
        Destroy, // Delete the returned object (for MB - Destroy)
    }

    // Generic pool managing a simple array stack.
    public class Pool<T> where T : class
    {
        private readonly OverflowBehavior _overflowBehavior;

        private   Func<T> _factory;
        protected T[]     _pool;
        protected int     _count;

        public Pool(OverflowBehavior overflowBehavior, Func<T> factory, int initialCapacity = 16)
        {
            _overflowBehavior = overflowBehavior;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _pool = new T[Math.Max(1, initialCapacity)];
            _count = 0;
        }

        /// <summary>
        /// In that case CreateInstance should be overriden
        /// </summary>
        public Pool(OverflowBehavior overflowBehavior, int initialCapacity = 16)
        {
            _overflowBehavior = overflowBehavior;

            _pool = new T[Math.Max(1, initialCapacity)];
            _count = 0;
        }

        protected virtual T CreateInstance() => _factory();

        public void SetCreateInstanceFactory(Func<T> factory)
        {
            _factory = factory;
        }

        protected void EnsureCapacityForPush()
        {
            if (_count < _pool.Length)
            {
                return;
            }

            switch (_overflowBehavior)
            {
                case OverflowBehavior.Expand:
                    var newSize = Math.Max(1, _pool.Length * 2);
                    Array.Resize(ref _pool, newSize);
                    break;

                case OverflowBehavior.Destroy:
                    // leave as is; push logic will handle by destroying
                    break;
            }
        }

        /// Return object to pool
        public virtual void Return(T item)
        {
            if (item == null)
            {
                return;
            }

            // If there is capacity - place
            if (_count < _pool.Length)
            {
                _pool[_count++] = item;
                OnReturnedToPool(item);
                return;
            }

            // No space
            switch (_overflowBehavior)
            {
                case OverflowBehavior.Expand:
                    EnsureCapacityForPush();
                    _pool[_count++] = item;
                    OnReturnedToPool(item);
                    break;

                case OverflowBehavior.Destroy:
                    OnDestroyReturned(item);
                    break;
            }
        }

        // Rent from pool (if empty, create new instance)
        public virtual T Rent()
        {
            if (_count > 0)
            {
                var idx = --_count;
                var item = _pool[idx];
                _pool[idx] = null;
                OnRentedFromPool(item);
                return item;
            }

            // pool empty -> create new
            var created = CreateInstance();
            OnRentedFromPool(created);
            return created;
        }

        /// Optional prewarm
        public void Prewarm(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            EnsureCapacityFor(amount);

            for (var i = 0; i < amount; ++i)
            {
                var instance = CreateInstance();
                Return(instance);
            }
        }

        protected void EnsureCapacityFor(int extra)
        {
            var needed = _count + extra;
            if (needed <= _pool.Length)
            {
                return;
            }

            var newSize = _pool.Length;
            while (newSize < needed)
            {
                newSize = Math.Max(1, newSize * 2);
            }

            Array.Resize(ref _pool, newSize);
        }

        protected virtual void OnRentedFromPool(T item)
        {
            if (item is IPoolable p)
            {
                p.OnRent();
            }
        }

        /// Hooks to override for behaviour (eg. enable/disable MB)
        protected virtual void OnReturnedToPool(T item)
        {
            if (item is IPoolable p)
            {
                p.OnReturn();
            }
        }

        protected virtual void OnDestroyReturned(T item)
        {
            // default: do nothing, subclass for MB should Destroy
        }
    }
}