using System;
using UnityEngine;

namespace TriggerTactics.Gameplay.Spawn
{
    public interface ISpawner<T> where T : IBaseSpawnable
    {
        event Action<T> OnSpawn;
        event Action<T> OnDespawn;

        void ClearSpawnedObjects();
        void Spawn(T spawnPrefab, bool setParent, Action<T> spawnedCallback, uint ownerID = 0);
        void Spawn(int spawnTypeID, bool setParent, Action<T> despawnedCallback, uint ownerID = 0);
        void Despawn(T despawnPrefab);
    }
}

