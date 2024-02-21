using FishNet.Managing;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    // Singleton pattern to ensure only one instance of BootstrapManager exists.
    private static BootstrapManager instance;
    private void Awake() => instance = this;

    // Serialized fields to be set in the Unity Editor.
    [SerializeField] private string menuName = "Menu"; // The name of the menu scene.
    [SerializeField] private NetworkManager _networkManager; // Reference to FishNet's NetworkManager.
    [SerializeField] private FishySteamworks.FishySteamworks _fishySteamworks; // Custom Steamworks integration.

    // Steamworks.NET callbacks for lobby events.
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    // Stores the current Steam Lobby ID.
    public static ulong CurrentLobbyID;

    private void Start()
    {
        // Initializes the callbacks with methods to handle specific Steam lobby events.
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    // Loads the menu scene additively.
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuName, LoadSceneMode.Additive);
    }

    // Static method to create a Steam lobby that is only visible to friends, with a maximum of 4 players.
    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    // Called when a lobby is successfully created. Sets lobby data and starts a network connection.
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        CurrentLobbyID = callback.m_ulSteamIDLobby;
        // Sets lobby data such as the host's address and lobby name.
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress", SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "name", SteamFriends.GetPersonaName() + "'s lobby");
        // Starts a FishNet connection as the host.
        _fishySteamworks.SetClientAddress(SteamUser.GetSteamID().ToString());
        _fishySteamworks.StartConnection(true);
        Debug.Log("Lobby creation was successful");
    }

    // Called when a game lobby join request is received. Joins the requested lobby.
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    // Called when successfully entered a lobby. Sets up the network connection based on lobby data.
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        // Notifies the MainMenuManager that a lobby has been entered and whether this client is the server.
        MainMenuManager.LobbyEntered(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name"), _networkManager.IsServer);

        // Sets client address from lobby data and starts a FishNet connection as a client.
        _fishySteamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress"));
        _fishySteamworks.StartConnection(false);
    }

    // Static method to join a lobby by SteamID. Checks if the lobby data can be requested before joining.
    public static void JoinByID(CSteamID steamID)
    {
        if (SteamMatchmaking.RequestLobbyData(steamID))
            SteamMatchmaking.JoinLobby(steamID);
        else
            Debug.Log("Failed to join lobby with ID: " + steamID.m_SteamID);
    }

    // Static method to leave the current lobby and stop network connections.
    public static void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        CurrentLobbyID = 0;

        // Stops the FishNet connection for both client and server, if applicable.
        instance._fishySteamworks.StopConnection(false);
        if (instance._networkManager.IsServer)
            instance._fishySteamworks.StopConnection(true);
    }
}
