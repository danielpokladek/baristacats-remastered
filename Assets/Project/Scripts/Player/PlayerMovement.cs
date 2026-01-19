using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody2D _rigidbody;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _movementSpeed = 10f;

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

        if (x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (x < 0)
        {
            _spriteRenderer.flipX = false;
        }
    }
}
