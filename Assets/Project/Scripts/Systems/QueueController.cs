using System.Threading.Tasks;
using PrimeTween;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

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

    private ObjectPool<CustomerController> _customerPool;

    private void Awake()
    {
        _customerPool = new(
            createFunc: CreateNewCustomer,
            actionOnGet: OnGetCustomer,
            actionOnRelease: OnReleaseCustomer,
            actionOnDestroy: OnDestroyCustomer,
            true,
            10
        );
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

    public CustomerController GetCustomer()
    {
        return _customerPool.Get();
    }

    public void ReturnCustomer(CustomerController customer)
    {
        _customerPool.Release(customer);
    }

    public Tween MoveToSpawn(CustomerController customer)
    {
        return customer.Movement.MoveTo(_spawnPosition);
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

    public async Task MoveToExit(CustomerController customer)
    {
        await customer.Movement.MoveTo(_exitPosition);

        ReturnCustomer(customer);
    }

    private CustomerController CreateNewCustomer()
    {
        var randomIndex = Random.Range(0, _customerPrefabs.Length);
        var customerPrefab = _customerPrefabs[randomIndex];

        var customer = Instantiate(customerPrefab, _spawnPosition, Quaternion.identity);
        customer.Setup();

        return customer;
    }

    private void OnGetCustomer(CustomerController customer)
    {
        customer.transform.position = _spawnPosition;
        customer.gameObject.SetActive(true);
    }

    private void OnReleaseCustomer(CustomerController customer)
    {
        customer.StopTimer();
        customer.gameObject.SetActive(false);
        customer.enabled = false;
    }

    private void OnDestroyCustomer(CustomerController customer)
    {
        Destroy(customer.gameObject);
    }

    private Vector2 GetPositionWithSpacing(Vector2 destination, int customerCount)
    {
        var newPos = destination;
        newPos.x += _customerGap * customerCount;

        return newPos;
    }
}
