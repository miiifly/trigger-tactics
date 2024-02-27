using System;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // Singleton instance to ensure only one MainMenuManager exists.
    private static MainMenuManager instance;

    // Serialized fields to reference UI elements set in the Unity Editor.
    [SerializeField] private GameObject _menuScreen, _lobbyScreen; // References to the menu and lobby UI screens.
    [SerializeField] private TMP_InputField _lobbyInput; // Input field for entering a lobby ID.

    [SerializeField] private TextMeshProUGUI _lobbyTitle, _lobbyIDText; // Text elements to display the lobby's title and ID.
    [SerializeField] private Button _startGameButton; // Button to start the game, visible only to the host.
    private void Awake() => instance = this; // Initializes the singleton instance.

    //[SerializeField]
    //private List<SceneAsset> scenes = new List<SceneAsset>();

    //[SerializeField]
    //private List<string> names = new List<string>();

    [SerializeField]
    private string _gameScenneName;

    //private void OnValidate()
    //{
    //    if(scenes.Count != names.Count)
    //    {
    //        names.Clear();
    //        foreach(SceneAsset asset in scenes)
    //        {
    //            names.Add(asset.name);
    //        }
    //    }
    //}

    private void Start()
    {
        OpenMainMenu(); // Opens the main menu screen on start.
    }

    public void CreateLobby()
    {
        BootstrapManager.CreateLobby(); // Calls BootstrapManager to create a Steam lobby.
    }

    public void OpenMainMenu()
    {
        CloseAllScreens(); // Closes all screens.
        _menuScreen.SetActive(true); // Activates the main menu screen.
    }

    public void OpenLobby()
    {
        CloseAllScreens(); // Closes all screens.
        _lobbyScreen.SetActive(true); // Activates the lobby screen.
    }

    // Static method called when a lobby has been successfully entered.
    public static void LobbyEntered(string lobbyName, bool isHost)
    {
        instance._lobbyTitle.text = lobbyName; // Sets the lobby title.
        instance._startGameButton.gameObject.SetActive(isHost); // Shows the start game button only to the host.
        instance._lobbyIDText.text = BootstrapManager.CurrentLobbyID.ToString(); // Displays the current lobby ID.
        instance.OpenLobby(); // Opens the lobby screen.
    }

    // Helper method to close all UI screens.
    void CloseAllScreens()
    {
        _menuScreen.SetActive(false);
        _lobbyScreen.SetActive(false);
    }

    public void JoinLobby()
    {
        // Converts the input text to a CSteamID and calls BootstrapManager to join the lobby.
        CSteamID steamID = new CSteamID(Convert.ToUInt64(_lobbyInput.text));
        BootstrapManager.JoinByID(steamID);
    }

    public void LeaveLobby()
    {
        BootstrapManager.LeaveLobby(); // Calls BootstrapManager to leave the current lobby.
        OpenMainMenu(); // Returns to the main menu screen.
    }

    public void StartGame()
    {
        // Calls BootstrapNetworkManager to change the network scene to "Game", closing the "Menu" scene.
        string[] scenesToClose = new string[] { "Menu" };
        BootstrapNetworkManager.ChangeNetworkScene(_gameScenneName, scenesToClose);
    }
}
