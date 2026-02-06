using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PaddleController : NetworkBehaviour
{
    [SerializeField] protected float moveSpeed = 8f;

    protected Rigidbody2D rb;

    // Owner writes, everyone reads
    private NetworkVariable<float> netY = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (IsOwner)
            netY.Value = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            float input = GetMoveInput();

            rb.velocity = new Vector2(0f, input * moveSpeed);

            netY.Value = rb.position.y;
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.position = new Vector2(rb.position.x, netY.Value);
        }
    }

    protected abstract float GetMoveInput();
}