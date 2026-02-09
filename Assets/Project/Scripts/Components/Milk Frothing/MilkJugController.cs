using UnityEngine;

public enum MilkJugDepth
{
    NONE,
    SHALLOW,
    DEEP,
}

public class MilkJugController : MonoBehaviour
{
    [Header("Positioning")]
    [SerializeField]
    private Vector2 _minPosition;

    [SerializeField]
    private Vector2 _maxPosition;

    [Header("Trigger Areas")]
    [SerializeField]
    private RectTransform _topArea;

    [SerializeField]
    private RectTransform _bottomArea;

    [SerializeField]
    private RectTransform _nozzlePoint;

    private readonly float _sensitivity = 1f;
    private float _moveProgress = 0f;

    private MilkJugDepth _currentJugDepth = MilkJugDepth.NONE;

    private Vector2 _moveInput;

    private void Start()
    {
        var actions = ControlsManager.FrothingActions;

        actions.Move.performed += ctx =>
        {
            _moveInput = ctx.ReadValue<Vector2>();
        };
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJugDepth();
    }

    private void HandleMovement()
    {
        float yDirection = Mathf.Clamp(_moveInput.y, -1, 1);
        float moveDelta = yDirection * _sensitivity * Time.deltaTime;
        _moveProgress += moveDelta;

        Vector2 newPos = Vector2.Lerp(_minPosition, _maxPosition, _moveProgress);
        transform.localPosition = new Vector3(newPos.x, newPos.y, transform.position.z);

        _moveInput = Vector2.zero;
    }

    private void HandleJugDepth()
    {
        var jugDepth = MilkJugDepth.NONE;

        if (RectTransformUtility.RectangleContainsScreenPoint(_topArea, _nozzlePoint.position))
        {
            jugDepth = MilkJugDepth.SHALLOW;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(_bottomArea, _nozzlePoint.position))
        {
            jugDepth = MilkJugDepth.DEEP;
        }

        if (jugDepth == _currentJugDepth)
            return;

        _currentJugDepth = jugDepth;
        MilkFrothingManager.OnJugDepthChange.Invoke(jugDepth);
    }
}
