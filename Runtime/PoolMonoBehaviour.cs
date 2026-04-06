using System;
using UnityEngine;

namespace _Source.Gameplay.Pools.Common
{
    public abstract class PoolMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected int              _initialCapacity  = 16;
        [SerializeField] protected OverflowBehavior _overflowBehavior = OverflowBehavior.Expand;
        [SerializeField] protected T                _prefab;

        [SerializeField, Tooltip("optional parent")]
        protected Transform _parentForPooled;

        [SerializeField] protected bool _deactivateOnReturn = true;

        private Pool<T> _pool;

        public void Prewarm(int amount) =>
            _pool.Prewarm(amount);

        public virtual T Rent()
        {
            var item = _pool.Rent();
            OnRentedFromPool(item);
            return item;
        }

        public virtual void Return(T item)
        {
            _pool.Return(item);

            switch (_overflowBehavior)
            {
                case OverflowBehavior.Expand:
                    OnReturnedToPool(item);
                    break;

                case OverflowBehavior.Destroy:
                    OnDestroyReturned(item);
                    break;
            }
        }

        protected virtual void Awake()
        {
            InitializePool(_overflowBehavior, CreateInstance, _initialCapacity);
        }

        protected void InitializePool(OverflowBehavior overflowBehavior, Func<T> createFactory, int initialCapacity = 16)
        {
            _overflowBehavior = overflowBehavior;
            _pool = new Pool<T>(
                _overflowBehavior,
                createFactory,
                initialCapacity
            );
        }

        /// Instantiate prefab
        protected virtual T CreateInstance()
        {
            if (_prefab == null)
            {
                return default;
            }

            var go = Instantiate(_prefab, _parentForPooled);
            go.gameObject.SetActive(true); // created items are active when rented
            return go;
        }

        protected virtual void OnRentedFromPool(T item)
        {
            /* no-op */
        }

        protected virtual void OnReturnedToPool(T item)
        {
            if (item == null)
            {
                return;
            }

            if (_deactivateOnReturn)
            {
                item.gameObject.SetActive(false);
            }

            // move to parent
            if (_parentForPooled != null)
            {
                item.transform.SetParent(_parentForPooled, false);
            }
        }

        protected virtual void OnDestroyReturned(T item)
        {
            if (item == null)
            {
                return;
            }

            Destroy(item.gameObject);
        }
    }
}