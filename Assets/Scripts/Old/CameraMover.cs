using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public static Camera PlayerCamera { private set; get; }
    public float Speed;

    private void Awake() => PlayerCamera = GetComponent<Camera>();

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalInput * Speed, verticalInput * Speed) * Time.deltaTime);
    }
}
