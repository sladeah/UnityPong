// PongNetworkSpawner.cs
using UnityEngine;
using Unity.Netcode;

public class PongNetworkSpawner : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NetworkObject leftPaddlePrefab;
    [SerializeField] private NetworkObject rightPaddlePrefab;
    [SerializeField] private NetworkObject ballPrefab;

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 leftSpawnPos = new Vector3(-7f, 0f, 0f);
    [SerializeField] private Vector3 rightSpawnPos = new Vector3(7f, 0f, 0f);
    [SerializeField] private Vector3 ballSpawnPos = Vector3.zero;

    private NetworkObject leftPaddleInstance;
    private NetworkObject rightPaddleInstance;
    private NetworkObject ballInstance;

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        if (!IsServer) return;

        // Spawn ONE ball on server. GameManager will freeze it until StartGame.
        if (ballInstance == null && ballPrefab != null)
        {
            ballInstance = Instantiate(ballPrefab, ballSpawnPos, Quaternion.identity);
            ballInstance.Spawn();
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

        base.OnDestroy();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        // Host gets left paddle
        if (clientId == NetworkManager.ServerClientId && leftPaddleInstance == null)
        {
            leftPaddleInstance = Instantiate(leftPaddlePrefab, leftSpawnPos, Quaternion.identity);
            leftPaddleInstance.SpawnWithOwnership(clientId);
            return;
        }

        // First remote client gets right paddle
        if (clientId != NetworkManager.ServerClientId && rightPaddleInstance == null)
        {
            rightPaddleInstance = Instantiate(rightPaddlePrefab, rightSpawnPos, Quaternion.identity);
            rightPaddleInstance.SpawnWithOwnership(clientId);
        }
    }
}