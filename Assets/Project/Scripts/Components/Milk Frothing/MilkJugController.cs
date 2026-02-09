using UnityEngine;

public class MilkJugController : MonoBehaviour
{
    [SerializeField]
    private Vector2 _minPos;

    [SerializeField]
    private Vector2 _maxPos;

    private readonly float _sensitivity = 3f;

    private Vector2 _moveInput;

    private float _moveProgress = 0f;

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
        float yDirection = Mathf.Clamp(_moveInput.y, -1, 1);
        float moveDelta = yDirection * _sensitivity * Time.deltaTime;
        _moveProgress += moveDelta;

        Vector2 newPos = Vector2.Lerp(_minPos, _maxPos, _moveProgress);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        _moveInput = Vector2.zero;
    }
}
