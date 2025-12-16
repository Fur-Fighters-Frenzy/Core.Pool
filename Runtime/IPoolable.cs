namespace _Source.Gameplay.Pools.Common
{
    public interface IPoolable
    {
        void OnRent(); // called when rented from pool

        void OnReturn(); // called when returned to pool
    }
}