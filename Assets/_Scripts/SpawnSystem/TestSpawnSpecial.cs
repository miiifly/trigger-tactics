using System.Collections;
using System.Collections.Generic;
using TriggerTactics.Gameplay.Special;
using UnityEngine;
using Zenject;

namespace TriggerTactics.Gameplay.Spawn
{
    public class TestSpawnSpecial : MonoBehaviour
    {
        [Inject]
        private ISpawner<ISpecialComponent> _spawner;
        [SerializeField]
        private SpecialType _specialType;

        private int _spawnID => _specialType.GetHashCode();

        private void Start()
        {

            SpawnObject();
        }
        void SpawnObject()
        {
            Debug.Log(_spawnID);
            _spawner.Spawn(_spawnID, true, SpawnInfo);
        }

        private void SpawnInfo(ISpecialComponent baseSpecial)
        {
            Debug.Log($"GameObject {baseSpecial.GetType().Name} . Was succeseful spawnned!!!");
        }
    }
}

