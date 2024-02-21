using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


namespace TriggerTactics.Gameplay
{
    public class ShotgunManager : MonoBehaviour/*, IShotgunManager*/
    {
        private List<bool> _shotgunAmmos;
        [SerializeField]
        private GameControllerSettings _shotgunSettings;

        private ShotgunAmmos _ammos;
        public ShotgunAmmos Ammos => _ammos;

        public event Action /*IShotgunManager.*/MagazineEmpty
        {
            add { _magazineEmpty += value; }
            remove { _magazineEmpty -= value; }
        }

        private Action _magazineEmpty;

        public void /*IShotgunManager.*/GenerateAmmon()
        {
            var totalPatrons = Random.Range(_shotgunSettings.MinAmmo, _shotgunSettings.MaxAmmo);
            var numEmpty = Random.Range(1, _shotgunSettings.MaxAmmo - _shotgunSettings.MinAmmo);
            var numReal = totalPatrons - numEmpty;

            _ammos = new ShotgunAmmos
            {
                EmptyPatrons = numEmpty,
                RealPatrons = numReal,
            };

            //TODO: Show object on Table
        }

        public void /*IShotgunManager.*/InitializeShotgun()
        {
            if (_ammos.RealPatrons <= 0 && _ammos.EmptyPatrons <= 0)
            {
                Debug.LogError("Ammo for shotgun not initialize!!!");
                return;
            }

            Load();
        }

        private void Load()
        {
            _shotgunAmmos = new List<bool>();

            for (int i = 0; i < _ammos.EmptyPatrons; i++)
            {
                _shotgunAmmos.Add(false);
            }
            for (int i = 0; i < _ammos.RealPatrons; i++)
            {
                _shotgunAmmos.Add(true);
            }

            ShuffleShotgun();
        }

        private void ShuffleShotgun()
        {
            for (int i = 0; i < _shotgunAmmos.Count; i++)
            {
                var temp = _shotgunAmmos[i];
                var randomIndex = Random.Range(i, _shotgunAmmos.Count);
                _shotgunAmmos[i] = _shotgunAmmos[randomIndex];
                _shotgunAmmos[randomIndex] = temp;
            }
        }

        public bool /*IShotgunManager.*/Shoot()
        {
            var action = _shotgunAmmos.First() ? true : false;
            if (action)
            {
                _ammos.RealPatrons--;
            }
            else
            {
                _ammos.EmptyPatrons--;
            }
            _shotgunAmmos.RemoveAt(0);
            CheckShotgunMagazine();
            return action;
        }

        private void CheckShotgunMagazine()
        {
            if (_shotgunAmmos.Count <= 0)
            {
                Debug.LogWarning("Shotgun is empty!");
                _shotgunAmmos = null;
                _magazineEmpty.Invoke();
            }
        }
    }

    public struct ShotgunAmmos
    {
        public int EmptyPatrons;
        public int RealPatrons;
    }
}