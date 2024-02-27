using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class BootstrapNetworkManager : NetworkBehaviour
{
    // Singleton instance to ensure only one BootstrapNetworkManager exists.
    private static BootstrapNetworkManager instance;
    private void Awake() => instance = this; // Initializes the singleton instance on Awake.

    // Static method to change the network scene for all connected clients.
    public static void ChangeNetworkScene(string sceneName, string[] scenesToClose)
    {
        instance.CloseScenes(scenesToClose); // Calls the instance method to close specified scenes.

        // Create a SceneLoadData object for the scene to load.
        SceneLoadData sld = new SceneLoadData(sceneName);
        // Get an array of all client connections.
        NetworkConnection[] conns = instance.ServerManager.Clients.Values.ToArray();
        // Load the specified scene across all connections.
        instance.SceneManager.LoadConnectionScenes(conns, sld);
    }

    // ServerRpc attribute indicates this method is a Remote Procedure Call that should be executed on the server.
    // RequireOwnership = false allows any client to call this method, not just the owner of the object.
    [ServerRpc(RequireOwnership = false)]
    void CloseScenes(string[] scenesToClose)
    {
        // Calls an ObserversRpc method to close specified scenes on all clients observing this object.
        CloseScenesObserver(scenesToClose);
    }

    // ObserversRpc attribute indicates this method is an RPC that will be executed on all clients observing this object.
    [ObserversRpc]
    void CloseScenesObserver(string[] scenesToClose)
    {
        // Iterates through each scene name in the provided array and unloads it asynchronously.
        foreach (var sceneName in scenesToClose)
        {
            Debug.Log($"Unloading scene: {sceneName}");
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
