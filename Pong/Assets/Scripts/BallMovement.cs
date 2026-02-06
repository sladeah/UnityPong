using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
public class BallMovement : NetworkBehaviour, ICollidable
{
    private float speed = 8f;
    private Vector2 direction = Vector2.right;

    private Rigidbody2D rb;

    public float Speed
    {
        get { return speed; }
        set
        {
            if (value < 0f)
                speed = 0f;
            else
                speed = value;
        }
    }

    public Vector2 Direction
    {
        get { return direction; }
        set
        {
            if (value != Vector2.zero)
                direction = value.normalized;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (IsServer)
        {
            Direction = Vector2.right;
            Speed = speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

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

    public void OnHit(Collision2D collision)
    {
    }
}