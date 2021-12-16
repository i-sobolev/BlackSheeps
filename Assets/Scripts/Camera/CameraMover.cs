using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private void Update() => Move();

    public void Move()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.ClampMagnitude(new Vector3(horizontalInput, verticalInput), 1) * _speed * Time.deltaTime);
    }
}
