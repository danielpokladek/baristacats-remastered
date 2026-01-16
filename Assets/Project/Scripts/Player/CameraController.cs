using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _playerCamera;

    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private Vector3 _cameraOffset = Vector3.zero;

    [SerializeField]
    private Vector2 _xMinMax = Vector2.zero;

    [SerializeField]
    private float _smoothTime = 0.3f;

    private Vector3 _velocity = Vector3.zero;

    private Transform _cameraTransform;

    private void Start()
    {
        _cameraTransform = _playerCamera.transform;
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = _playerTransform.TransformPoint(_cameraOffset);
        Vector3 newPosition = Vector3.SmoothDamp(
            _cameraTransform.position,
            targetPosition,
            ref _velocity,
            _smoothTime
        );

        newPosition.x = Mathf.Clamp(newPosition.x, _xMinMax.x, _xMinMax.y);

        _cameraTransform.position = newPosition;
    }
}
