using UnityEngine;

public class LeftPaddleController : PaddleController
{
    protected override float GetMoveInput()
    {
        return Input.GetAxis("LeftPaddle");
    }
}