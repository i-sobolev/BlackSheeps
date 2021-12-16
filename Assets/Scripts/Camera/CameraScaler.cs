using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField] private float _scaleSpeed = 0.01f;
    [Header("Camera size range")]
    [SerializeField] [InspectorName("Min")] private float _cameraSizeRangeMin;
    [SerializeField] [InspectorName("Max")] private float _cameraSizeRangeMax;

    private Camera _camera;

    private float _currentSizeLerp = 1;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = GetCameraSizeByLerp(1);
    }

    private void Update()
    {
        if (Input.mouseScrollDelta == Vector2.up || Input.mouseScrollDelta == Vector2.down)
        {
            _currentSizeLerp += Input.mouseScrollDelta.y * _scaleSpeed;
            _currentSizeLerp = Mathf.Clamp01(_currentSizeLerp);
            _camera.orthographicSize = GetCameraSizeByLerp(_currentSizeLerp);
        }
    }

    private float GetCameraSizeByLerp(float lerpValue)
    {
        return Mathf.Lerp(_cameraSizeRangeMax, _cameraSizeRangeMin, lerpValue);
    }
}