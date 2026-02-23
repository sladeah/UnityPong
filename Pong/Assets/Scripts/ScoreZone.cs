using UnityEngine;
using Unity.Netcode;

public class ScoreZone : MonoBehaviour
{
    private GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        // Only the server/host should award points
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

        if (gm == null) gm = FindObjectOfType<GameManager>();
        if (gm == null) return;

        // Tag-based scoring
        if (CompareTag("LeftScoreZone"))
            gm.ScoreRightPoint(); // ball hit left goal -> right player scores
        else if (CompareTag("RightScoreZone"))
            gm.ScoreLeftPoint();  // ball hit right goal -> left player scores
    }
}