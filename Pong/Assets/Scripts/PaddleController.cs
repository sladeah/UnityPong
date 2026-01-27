using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 8f;

    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        float input = GetMoveInput();
        rb.velocity = new Vector2(0f, input * moveSpeed);
    }

    protected virtual float GetMoveInput()
    {
        return 0f;
    }
}