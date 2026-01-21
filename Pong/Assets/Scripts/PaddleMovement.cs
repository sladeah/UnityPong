using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = Vector3.up * verticalInput * speed * Time.deltaTime;
        transform.position += movement;
    }
}