using PrimeTween;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class QueueController : MonoBehaviour
{
    [SerializeField]
    private Vector3 _spawnPosition;

    [SerializeField]
    private Vector3 _orderPosition;

    [SerializeField]
    private Vector3 _payPosition;

    [SerializeField]
    private Vector3 _exitPosition;

    [SerializeField]
    private float _customerGap = 2f;

    [SerializeField]
    private CustomerController[] _customerPrefabs;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _doorSound;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public CustomerController GetCustomer(
        ApplicationManager appManager,
        DifficultyController difficultyController
    )
    {
        var randomIndex = Random.Range(0, _customerPrefabs.Length);
        var customerPrefab = _customerPrefabs[randomIndex];

        // TODO: Pool customers instead of creating new instance each time.
        var customer = Instantiate(customerPrefab, _spawnPosition, Quaternion.identity);
        customer.Setup(appManager, difficultyController);

        _audioSource.PlayOneShot(_doorSound);

        return customer;
    }

    public Tween MoveToOrderDesk(CustomerController customer, int queueIndex)
    {
        var positionInQueue = GetPositionWithSpacing(_orderPosition, queueIndex);
        return customer.Movement.MoveTo(positionInQueue);
    }

    public Tween MoveToPayDesk(CustomerController customer, int queueIndex)
    {
        var positionInQueue = GetPositionWithSpacing(_payPosition, queueIndex);
        return customer.Movement.MoveTo(positionInQueue);
    }

    public Tween MoveToExit(CustomerController customer)
    {
        return customer.Movement.MoveTo(_exitPosition);
    }

    private Vector2 GetPositionWithSpacing(Vector2 destination, int customerCount)
    {
        var newPos = destination;
        newPos.x += _customerGap * customerCount;

        return newPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Handles.Label(_spawnPosition, "Spawn Position");
        Gizmos.DrawWireSphere(_spawnPosition, 0.5f);

        Gizmos.color = Color.blue;
        Handles.Label(_orderPosition, "Order Position");
        Gizmos.DrawWireSphere(_orderPosition, 0.5f);

        Gizmos.color = Color.blue;
        Handles.Label(_payPosition, "Pay Position");
        Gizmos.DrawWireSphere(_payPosition, 0.5f);

        Gizmos.color = Color.red;
        Handles.Label(_exitPosition, "Exit Position");
        Gizmos.DrawWireSphere(_exitPosition, 0.5f);
    }
}
