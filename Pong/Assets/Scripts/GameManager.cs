// GameManager.cs
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class GameManager : NetworkBehaviour
{
    [Header("Win Condition")]
    [SerializeField] private int pointsToWin = 5;

    [Header("Scene References")]
    [SerializeField] private Rigidbody2D ballRb;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private float serveSpeed = 8f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI leftScoreText;
    [SerializeField] private TextMeshProUGUI rightScoreText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private GameObject startButtonObject;

    private readonly NetworkVariable<int> leftScore = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<int> rightScore = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<bool> gameOver = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Gate ball + gameplay until host starts
    private readonly NetworkVariable<bool> gameStarted = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool ballInitialized;

    public bool IsGameStarted() => gameStarted.Value;

    public override void OnNetworkSpawn()
    {
        // Server initializes authoritative state
        if (IsServer)
        {
            gameStarted.Value = false;
            gameOver.Value = false;
            leftScore.Value = 0;
            rightScore.Value = 0;
        }

        // UI updates when values change
        leftScore.OnValueChanged += (_, __) => RefreshUI();
        rightScore.OnValueChanged += (_, __) => RefreshUI();
        gameOver.OnValueChanged += (_, __) => RefreshUI();
        gameStarted.OnValueChanged += (_, __) => RefreshUI();

        if (winText != null) winText.gameObject.SetActive(false);

        RefreshUI();
    }

    private void LateUpdate()
    {
        EnsureBallRefs();

        // As soon as server sees the spawned ball for the first time, freeze & center it.
        if (IsServer && ballRb != null && ballTransform != null && !ballInitialized)
        {
            ballTransform.position = Vector3.zero;
            StopBall();
            ballInitialized = true;
        }
    }

    private void EnsureBallRefs()
    {
        if (ballRb != null && ballTransform != null) return;

        GameObject ballObj = GameObject.FindGameObjectWithTag("Ball");
        if (ballObj == null) return;

        ballTransform = ballObj.transform;
        ballRb = ballObj.GetComponent<Rigidbody2D>();
        ballInitialized = false;
    }

    private void RefreshUI()
    {
        if (leftScoreText != null) leftScoreText.text = leftScore.Value.ToString();
        if (rightScoreText != null) rightScoreText.text = rightScore.Value.ToString();

        if (winText != null)
        {
            if (gameOver.Value)
            {
                winText.gameObject.SetActive(true);
                if (leftScore.Value >= pointsToWin) winText.text = "Left Player Wins!";
                else if (rightScore.Value >= pointsToWin) winText.text = "Right Player Wins!";
                else winText.text = "Game Over!";
            }
            else
            {
                winText.gameObject.SetActive(false);
            }
        }

        // Start button: show only on host, and only before game starts.
        if (startButtonObject != null)
            startButtonObject.SetActive(IsServer && !gameStarted.Value);
    }

    // IMPORTANT: Button can call this from host OR client.
    // If a client clicks it, we forward to the server via ServerRpc.
    public void StartGame()
    {
Debug.Log($"[StartGame] CLICKED. IsServer={IsServer} IsHost={IsHost} IsSpawned={IsSpawned}");
        if (IsServer) StartGameInternal();
        else StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(ServerRpcParams rpcParams = default)
    {
        StartGameInternal();
    }

    private void StartGameInternal()
    {
        if (!IsServer) return;

        EnsureBallRefs();
        if (ballRb == null || ballTransform == null) return; // ball not spawned yet

        // Reset match state
        leftScore.Value = 0;
        rightScore.Value = 0;
        gameOver.Value = false;
        gameStarted.Value = true;

        if (winText != null) winText.gameObject.SetActive(false);

        // Serve
        bool serveRight = Random.value > 0.5f;
        ResetBallAndServe(serveRight);

        RefreshUI();
    }

    public void ScoreLeftPoint()
    {
        if (!IsServer || gameOver.Value || !gameStarted.Value) return;

        leftScore.Value++;
        CheckWinAndMaybeEnd();

        if (!gameOver.Value)
            ResetBallAndServe(directionToRight: true);
    }

    public void ScoreRightPoint()
    {
        if (!IsServer || gameOver.Value || !gameStarted.Value) return;

        rightScore.Value++;
        CheckWinAndMaybeEnd();

        if (!gameOver.Value)
            ResetBallAndServe(directionToRight: false);
    }

    private void CheckWinAndMaybeEnd()
    {
        if (leftScore.Value >= pointsToWin || rightScore.Value >= pointsToWin)
        {
            gameOver.Value = true;
            StopBall();
        }
    }

    private void StopBall()
    {
        if (ballRb == null) return;

        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        // Prevent BallMovement from re-applying velocity
        BallMovement bm = ballRb.GetComponent<BallMovement>();
        if (bm != null) bm.Speed = 0f;
    }

    private void ResetBallAndServe(bool directionToRight)
    {
        if (ballTransform == null || ballRb == null) return;

        ballTransform.position = Vector3.zero;
        ballRb.velocity = Vector2.zero;

        Vector2 dir = directionToRight ? Vector2.right : Vector2.left;

        // Update BallMovement state so it drives movement
        BallMovement bm = ballRb.GetComponent<BallMovement>();
        if (bm != null)
        {
            bm.Direction = dir;
            bm.Speed = serveSpeed;
        }

        // Immediate kick
        ballRb.velocity = dir * serveSpeed;
    }
}