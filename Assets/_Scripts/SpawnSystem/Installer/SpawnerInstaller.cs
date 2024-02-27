using System.Collections;
using System.Collections.Generic;
using TriggerTactics.Gameplay.Special;
using UnityEngine;
using Zenject;

namespace TriggerTactics.Gameplay.Spawn
{
    public class SpawnerInstaller : MonoInstaller
    {
        [SerializeField]
        private Transform _specialParent;
        [SerializeField]
        private SpecialPreset _specialPreset;

        public override void InstallBindings()
        {
            Container.BindInstance(_specialPreset);

            var specialSpawner = new SpecialSpawner(_specialParent, Container, _specialPreset.Specials, DestroySpawnedObjects);

            Container.Bind<ISpawner<ISpecialComponent>>().FromInstance(specialSpawner).AsSingle().NonLazy();
        }

        private void DestroySpawnedObjects<T>(IEnumerable<T> objToDestroy) where T : IBaseSpawnable
        {
            foreach (var obj in objToDestroy)
            {
                if (obj?.GameObject != null)
                {
                    Destroy(obj.GameObject);
                }
            }
        }
    }
}

