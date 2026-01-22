using UnityEngine;

public class MilkJugController : MonoBehaviour
{
    [SerializeField]
    private Vector2 _minPos;

    [SerializeField]
    private Vector2 _maxPos;

    private readonly float _mouseSensitivity = 0.5f;

    private Rigidbody2D _rigidbody;

    private Vector2 _moveInput;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        var actions = ControlsManager.FrothingActions;

        actions.Move.performed += ctx =>
        {
            _moveInput = ctx.ReadValue<Vector2>();
            // var delta = ctx.ReadValue<Vector2>();
            // delta = delta.normalized;

            // var pos = transform.position;
            // var newPos = new Vector3(
            //     pos.x + delta.x * (Time.deltaTime * _mouseSensitivity),
            //     pos.y + delta.y * (Time.deltaTime * _mouseSensitivity),
            //     pos.z
            // );

            // newPos.x = Mathf.Clamp(newPos.x, _minPos.x, _maxPos.x);
            // newPos.y = Mathf.Clamp(newPos.y, _minPos.y, _maxPos.y);

            // transform.transform.position = newPos;
        };
    }

    private void FixedUpdate()
    {
        Vector2 delta = _moveInput * _mouseSensitivity * Time.fixedDeltaTime;
        Vector2 targetPos = _rigidbody.position + delta;

        targetPos.x = Mathf.Clamp(targetPos.x, _minPos.x, _maxPos.x);
        targetPos.y = Mathf.Clamp(targetPos.y, _minPos.y, _maxPos.y);

        _rigidbody.MovePosition(targetPos);
        _moveInput = Vector2.zero;
    }
}
