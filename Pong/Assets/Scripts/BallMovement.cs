using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed, speed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
    }
}