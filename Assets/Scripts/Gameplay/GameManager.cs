using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using TMPro;
using TriggerTactics.Gameplay.Special;
using UnityEngine;
using Zenject;

namespace TriggerTactics.Gameplay
{
    public class GameManager : NetworkBehaviour
    {
        protected static GameManager instance;

        [SerializeField] private ShotgunManager _shotgunManager;
        [SerializeField] private SpecialManager _specialManager;

        [SyncVar(OnChange = nameof(OnStateChange))] private GameStates _gameStates;
        private GameStates _previousPlayerTurn;


        [SerializeField] private TextMeshProUGUI _emptyPatron;
        [SerializeField] private TextMeshProUGUI _realPatron;
        [SerializeField] private TextMeshProUGUI _action;
        [SerializeField] private TextMeshProUGUI _stateText;

        private string _shotgunEmptyPatrons = "EmptyPatrons: ";
        private string _shotgunRealPatrons = "RealPatrons: ";

        [SyncVar] private string _realPatronsCount;
        [SyncVar] private string _emptyPatronsCount;
        [SyncVar(OnChange =nameof(ActionUI))]private string _actionText;

        private int _player1HP = 3;
        private int _player2HP = 3;
        private int _targetToShoot;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            _shotgunManager.MagazineEmpty += RoundEnd;
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            if (IsServer)
                ChangeState(GameStates.RoundStart);
        }

        private void ChangeState(GameStates newState)
        {
            _gameStates = newState;

            switch(newState)
            {
                case GameStates.None:
                    break;
                case GameStates.RoundStart:
                    RoundStart();
                    break;
                case GameStates.Player1Tunr:
                    StartPlayer1Turn();
                    break;
                case GameStates.Player2Tunr:
                    StartPlayer2Turn();
                    break;
                case GameStates.NextPlayerTurn:
                    NextPlayerTurn();
                    break;
                case GameStates.RoundEnd:
                    RoundEnd();
                    break;
                case GameStates.GameEnd:
                    GameEnd();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        public static void PlayerSpecial(int playerID)
        {
            instance.PlayerSpecialServer(playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerSpecialServer(int playerID)
        {
            //_spellManager.SelectSpell();
        }

        public static void PlayerSelectTarget(int playerID)
        {
            instance.PlayerSelectTargetServer(playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerSelectTargetServer(int playerID)
        {
            _targetToShoot = _targetToShoot == 0 ? 1 : 0;
            _actionText = $"Player \"{playerID}\" set target to shoot \"{_targetToShoot}\" .";
            //Debug.Log($"Player \"{playerID}\" set target to shoot \"{_targetToShoot}\" .");
            UpdateActionUI();
        }

        public static void PlayerShoot(int playerID)
        {
            instance.PlayerShootServer(playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerShootServer(int playerID)
        {
            int targetHP;
            if (_shotgunManager.Shoot())
            {
                if(_targetToShoot == 0)
                {
                    _player1HP--;
                    targetHP = _player1HP;
                }
                else
                {
                    _player2HP--;
                    targetHP = _player2HP;
                }
                instance._actionText = $"Player \"{playerID}\" shoot. Target player \"{_targetToShoot}\" now have \"{targetHP}\" HP.";
            }
            else
            {
                instance._actionText = $"Player \"{playerID}\" shoot. But it`s a empty patron.";
            }
            UpdatePatronUI();

            //Debug.Log($"Player \"{playerID}\" shoot. Target player \"{_targetToShoot}\" now have \"{targetHP}\" .");
            UpdateActionUI();
            if (_player1HP == 0 || _player2HP == 0)
            {
                instance.ChangeState(GameStates.GameEnd);
            }
            else
            {
                instance.ChangeState(GameStates.NextPlayerTurn);
            }
        }

        public static void UpdateUIPlayer()
        {
            instance.UpdatePatronUI();
            instance.UpdateActionUI();
            instance.UpdateStateUI();
        }

        private void UpdatePatronUI()
        {
            _realPatronsCount = _shotgunRealPatrons + _shotgunManager.Ammos.RealPatrons.ToString();
            _realPatron.text = _realPatronsCount;
            _emptyPatronsCount = _shotgunEmptyPatrons + _shotgunManager.Ammos.EmptyPatrons.ToString();
            _emptyPatron.text = _emptyPatronsCount;
        }

        private void UpdateStateUI()
        {
            _stateText.text = $"State Changes : {instance._gameStates}";
        }

        private void ActionUI(string _previusText, string _newText, bool asServer)
        {
            UpdateActionUI();
        }
        private void UpdateActionUI()
        {
            _action.text = _actionText;
        }

        public static bool IsMyTurn(int playerID)
        {
            if (playerID == 0 && instance._gameStates == GameStates.Player1Tunr || playerID == 1 && instance._gameStates == GameStates.Player2Tunr)
                return true;
            return false;
        }

        private void StartPlayer1Turn()
        {
            //Debug.Log($"Playr \"1\" turn.");
            _previousPlayerTurn = GameStates.Player1Tunr;
            _targetToShoot = 1;
            UpdateActionUI();
        }

        private void StartPlayer2Turn()
        {
            //Debug.Log($"Playr \"2\" turn.");
            _previousPlayerTurn = GameStates.Player2Tunr;
            _targetToShoot = 0;
            UpdateActionUI();
        }

        private void RoundStart()
        {
            var playerTurn = /*Random.Range(0, 2)*/ 0;
            _specialManager.FillingPlayersSpecials();
            _shotgunManager.GenerateAmmon();
            UpdatePatronUI();
            _shotgunManager.InitializeShotgun();
            switch (playerTurn)
            {
                case 0:
                    ChangeState(GameStates.Player1Tunr);
                    break;
                case 1:
                    ChangeState(GameStates.Player2Tunr);
                    break;
            }
        }

        private void NextPlayerTurn()
        {
            switch (_previousPlayerTurn)
            {
                case GameStates.Player1Tunr:
                    ChangeState(GameStates.Player2Tunr);
                    break;
                case GameStates.Player2Tunr:
                    ChangeState(GameStates.Player1Tunr);
                    break;
            }
        }

        private void RoundEnd()
        {
            instance.ChangeState(GameStates.RoundStart);
        }

        private void GameEnd()
        {
            if(_player1HP <= 0)
            {
                _actionText = "Player 2 win !!!";
                //Debug.Log("Player 2 win !!!");
            }
            else
            {
                _actionText = "Player 1 win !!!";
                //Debug.Log("Player 1 win !!!");
            }

            UpdateActionUI();
        }

        private void OnStateChange(GameStates oldState, GameStates newState, bool asServer)
        {
            _stateText.text = $"State Changes : {instance._gameStates}";
        }

    }

    enum GameStates
    {
        None = 0,
        RoundStart = 1,
        Player1Tunr = 2,
        Player2Tunr = 3,
        NextPlayerTurn = 4,
        RoundEnd = 5,
        GameEnd = 6
    }
}

