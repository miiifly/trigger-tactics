using System;
using System.Collections;
using System.Collections.Generic;
using TriggerTactics.Gameplay.Spawn;
using TriggerTactics.Gameplay.Special;
using UnityEngine;
using Zenject;

namespace TriggerTactics.Gameplay.Spawn
{
    public class SpecialSpawner : BaseSpawner<ISpecialComponent>
    {
        public SpecialSpawner(Transform spawnParent,
        DiContainer container,
        IEnumerable<ISpecialComponent> spawnables,
        Action<IEnumerable<ISpecialComponent>> clearAction)
        : base(spawnParent, container, spawnables, clearAction)
        { }

        protected override void Spawn(ISpecialComponent spawnableComponent, bool setParent, Action<ISpecialComponent> spawnedCallback)
        {
            base.Spawn(spawnableComponent, setParent, (special) =>
            {
                spawnedCallback.Invoke(special);
            });
        }
    }
}

