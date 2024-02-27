using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriggerTactics.Gameplay
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TriggerTactics/Settings/GameSettings")]
    public class GameControllerSettings : ScriptableObject
    {
        [SerializeField]
        private int _minAmmo = 5;
        [SerializeField]
        private int _maxAmmo = 8;
        [SerializeField]
        private int _playerHP = 3;
        [SerializeField]
        private int _ammoDamage = 1;

        public int PlayerHP => _playerHP;
        public int AmmoDamage => _ammoDamage;
        public int MinAmmo => _minAmmo;
        public int MaxAmmo => _maxAmmo;
    }
}

