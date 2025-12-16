namespace _Source.Gameplay.Pools.Common
{
    public interface IKeyed<TKey>
    {
        TKey Key { get; }
    }
}