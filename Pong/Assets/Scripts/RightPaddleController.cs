using UnityEngine;

public class RightPaddleController : PaddleController, ICollidable
{
    protected override float GetMoveInput()
    {
        return Input.GetAxis("RightPaddle");
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