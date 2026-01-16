using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private float _startDelay = 0f;

    [SerializeField]
    private float _moveSpeed = 10f;

    [SerializeField]
    private Vector3 _positionOne = new Vector3(0, 0, 0);

    [SerializeField]
    private Vector3 _positionTwo = new Vector3(0, 0, 0);

    [SerializeField]
    private Vector2 _destinationWaitMinMax = new Vector2(2f, 2f);

    [SerializeField]
    private int _sameDirectionThreshold = 2;

    private SpriteRenderer _renderer;

    private Vector3 _targetPosition;

    private int _sameDirectionCounter;

    private float _waitAtDestination;
    private float _waitTimer;
    private float _scaleX;
    private float _scaleY;

    private bool _isFirstDirectionPick = true;
    private bool _previousMoveToRight;
    private bool _isWaiting;

    private void Start()
    {
        this.gameObject.SetActive(false);

        _renderer = this.GetComponent<SpriteRenderer>();
        _scaleX = transform.localScale.x;
        _scaleY = transform.localScale.y;

        if (_startDelay == 0)
        {
            PickNewDestination();
            return;
        }

        Invoke("PickNewDestination", _startDelay);
    }

    private void Update()
    {
        if (_isWaiting)
        {
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= _waitAtDestination)
            {
                _isWaiting = false;
            }

            return;
        }

        HandleCarMove();
    }

    private void HandleCarMove()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            _moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            PickNewDestination();
        }
    }

    private void PickNewDestination()
    {
        if (_isFirstDirectionPick)
        {
            _isFirstDirectionPick = false;
            this.gameObject.SetActive(true);
        }

        var moveToRight = Random.value >= 0.5 ? true : false;

        if (_previousMoveToRight == moveToRight)
        {
            if (_sameDirectionCounter >= _sameDirectionThreshold)
            {
                moveToRight = !moveToRight;
            }
            else
            {
                _sameDirectionCounter++;
            }
        }

        _previousMoveToRight = moveToRight;

        transform.position = moveToRight ? _positionOne : _positionTwo;
        _targetPosition = moveToRight ? _positionTwo : _positionOne;

        if (moveToRight)
        {
            transform.localScale = new Vector3(_scaleX, _scaleY, 1);
            _renderer.sortingOrder = -5;
        }
        else
        {
            transform.localScale = new Vector3(-_scaleX, _scaleY, 1);
            _renderer.sortingOrder = -6;
        }

        _waitAtDestination = Random.Range(_destinationWaitMinMax.x, _destinationWaitMinMax.y);

        _waitTimer = 0f;
        _isWaiting = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_positionOne, 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_positionTwo, 0.5f);
    }
#endif
}
