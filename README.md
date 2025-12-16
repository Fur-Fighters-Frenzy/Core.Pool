# Core.Pool

Lightweight object pooling utilities for Unity.

> **Status:** WIP

## What’s inside

- **Pool\<T\>** — generic array-stack pool.
  - **OverflowBehavior**
    - `Expand`: grows internal array (x2).
    - `Destroy`: refuses extra returns when full (delegates disposal to a hook).
- **IPoolable** — optional lifecycle callbacks:
  - `OnRent()` is called when an object is rented.
  - `OnReturn()` is called when returned to the pool.
- **PoolMonoBehaviour\<T\>** — prefab-based pool for `MonoBehaviour`:
  - Instantiates from prefab.
  - Optional parent for pooled objects.
  - Optional deactivation on return.
  - Can destroy returned items when overflow behavior is `Destroy`.
- **TrackedPool\<TKey, TValue\>** — tracks currently rented/spawned objects by key:
  - `TValue : IKeyed<TKey>`
  - `TryGetSpawned(key, out value)` for fast lookup.

## Notes

- `Pool<T>` is engine-agnostic (plain C#), while `PoolMonoBehaviour<T>` is Unity-specific.
- `TrackedPool` is meant for “spawned instances tracking” (projectiles, enemies, etc.).

---

# Part of the Core Project

This package is part of the **Core** project, which consists of multiple Unity packages.
See the full project here: [Core](https://github.com/Fur-Fighters-Frenzy/Core)
