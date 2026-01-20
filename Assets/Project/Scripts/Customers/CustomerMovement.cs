using UnityEngine;
using UnityEngine.Events;

public class CustomerMovement : MonoBehaviour
{
    public UnityEvent<CustomerController> OnArrived;

    [SerializeField]
    private float _movementSpeed;

    private CustomerController _customerController;
    private Vector3 _destination;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _customerController = GetComponent<CustomerController>();
        _rigidbody = GetComponent<Rigidbody2D>();

        enabled = false;
    }

    public void MoveTo(Vector2 position)
    {
        _destination = position;

        enabled = true;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            _destination,
            _movementSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, _destination) < 0.1f)
        {
            OnArrived.Invoke(_customerController);
            enabled = false;
        }
    }
}
