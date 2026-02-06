using UnityEngine;
using Unity.Netcode;

public class LeftPaddleController : PaddleController, ICollidable
{
    protected override float GetMoveInput()
    {
        if (!IsOwner) return 0f;

        return Input.GetAxis("LeftPaddle");
    }

    public void OnHit(Collision2D collision)
    {
        BallMovement ball = collision.otherCollider.GetComponent<BallMovement>();
        if (ball != null)
        {
            ball.Direction = new Vector2(-ball.Direction.x, ball.Direction.y);
        }
    }
}