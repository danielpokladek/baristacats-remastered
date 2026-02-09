using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody2D _rigidbody;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Transform _bodyTransform;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _movementSpeed = 10f;

    private bool _isFacingRight = false;

    private void Start()
    {
        _playerActions = ControlsManager.PlayerActions;
    }

    private InputSystem_Actions.PlayerActions _playerActions;

    private void Update()
    {
        HandleMove(_playerActions.Move.ReadValue<Vector2>().x);
    }

    private void HandleMove(float x)
    {
        _rigidbody.linearVelocity = new Vector2(x * _movementSpeed, 0);

        _animator.SetBool("IsWalking", x != 0);

        bool needsDirectionFlip = (x > 0 && !_isFacingRight) || (x < 0 && _isFacingRight);

        if (!needsDirectionFlip)
            return;

        Vector3 localScale = _bodyTransform.localScale;
        localScale.x *= -1;

        _bodyTransform.localScale = localScale;
        _isFacingRight = x > 0;
    }
}
