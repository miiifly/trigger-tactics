using System;
using System.Collections;
using System.Collections.Generic;
using TriggerTactics.Gameplay.Spawn;
using UnityEngine;
using Zenject;

namespace TriggerTactics.Gameplay.Special
{
    public class SpecialManager : MonoBehaviour
    {
        [Inject]
        private ISpawner<BaseSpecial> _specialSpawner;

        [SerializeField]
        private List<SpecialType> _firstPlayerSpecials = new List<SpecialType>();
        [SerializeField]
        private List<SpecialType> _secondPlayerSpecials = new List<SpecialType>();

        private int _currentRound = 1;
        private int _maxSpecialsPerRound = 3;

        private Dictionary<SpecialType, float> _probabilities = new Dictionary<SpecialType, float>()
        {
            {SpecialType.ShowAmmo, 0.2f},
            {SpecialType.Discharge, 0.3f},
            {SpecialType.Heal, 0.1f},
            {SpecialType.BlockTurn, 0.15f},
            {SpecialType.DoubleDamage, 0.25f}
        };

        private Action<int> _healAction;
        public event Action<int> HealPlayer
        {
            add { _healAction += value; }
            remove { _healAction -= value; } 
        }

        private Action<int> _blockTurnAction;
        public event Action<int> BlockTurn
        {
            add { _blockTurnAction += value; }
            remove { _blockTurnAction -= value; }
        }

        private Action _doubleDamageAction;
        public event Action DoubleDamage
        {
            add { _doubleDamageAction += value; }
            remove { _doubleDamageAction -= value; }
        }

        private Action _showAmmoAction;
        public event Action ShowAmmo
        {
            add { _showAmmoAction += value; }
            remove { _showAmmoAction -= value; }
        }

        private Action _dischargeAction;
        public event Action Discharge
        {
            add { _dischargeAction += value; }
            remove { _dischargeAction -= value; }
        }

        public void FillingPlayersSpecials()
        {
            _firstPlayerSpecials.Clear();
            _secondPlayerSpecials.Clear();

            for (int i = 0; i < 2; i++)
            {
                List<SpecialType> playerSpecials = (i == 0) ? _firstPlayerSpecials : _secondPlayerSpecials;

                for (int j = 0; j < _maxSpecialsPerRound * _currentRound; j++)
                {
                    foreach (var kvp in _probabilities)
                    {
                        if (UnityEngine.Random.value < kvp.Value)
                        {
                            playerSpecials.Add(kvp.Key);
                            break;
                        }
                    }
                }
            }
            _currentRound++;
        }

        public void SelectPlayerSpecial(int playerID, SpecialType selectedSpecial)
        {
            List<SpecialType> playerSpecials = (playerID == 0) ? _firstPlayerSpecials : _secondPlayerSpecials;

            foreach (var special in playerSpecials)
            {
                if(special == selectedSpecial)
                {
                    ChooseSpecial(playerID, special);
                }
            }
        }

        private void ChooseSpecial(int playerID,SpecialType selectedSpecial)
        {
            switch (selectedSpecial)
            {
                case SpecialType.Heal:
                    _healAction.Invoke(playerID);
                    break;
                case SpecialType.DoubleDamage:
                    _doubleDamageAction.Invoke();
                    break;
                case SpecialType.Discharge:
                    _dischargeAction.Invoke();
                    break;
                case SpecialType.ShowAmmo:
                    _showAmmoAction.Invoke();
                    break;
                case SpecialType.BlockTurn:
                    _blockTurnAction.Invoke(playerID != 0 ? 1 : 0);
                    break;
            }
        }
    }

    public enum SpecialType
    {
        None = 0,
        ShowAmmo = 1,
        Discharge = 2,
        Heal = 3,
        BlockTurn = 4,
        DoubleDamage = 5
    }
}