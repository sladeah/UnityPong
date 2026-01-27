using UnityEngine;

public class RightPaddleController : PaddleController
{
    protected override float GetMoveInput()
    {
        return Input.GetAxis("RightPaddle");
    }
}