// BallMovement.cs
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
public class BallMovement : NetworkBehaviour, ICollidable
{
    [SerializeField] private float speed = 8f;
    private Vector2 direction = Vector2.right;

    private Rigidbody2D rb;
    private GameManager gm;

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0f, value);
    }

    public Vector2 Direction
    {
        get => direction;
        set
        {
            if (value != Vector2.zero)
                direction = value.normalized;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    public override void OnNetworkSpawn()
    {
        rb.velocity = Vector2.zero;
        gm = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        // Re-acquire in case of timing
        if (gm == null) gm = FindObjectOfType<GameManager>();

        // Don't move until the match starts
        if (gm == null || !gm.IsGameStarted())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        if (collidable != null)
        {
            collidable.OnHit(collision);
        }
    }

    public void OnHit(Collision2D collision) { }
}