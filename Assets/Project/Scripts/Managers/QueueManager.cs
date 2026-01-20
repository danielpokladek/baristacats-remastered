using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance { get; private set; }

    [HideInInspector]
    public UnityEvent OnOrderQueueUpdated;

    [HideInInspector]
    public UnityEvent OnPayQueueUpdated;

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

    private Queue<CustomerController> _orderQueue;
    private Queue<CustomerController> _payQueue;

    private GameManager _game;

    public bool HasCustomersAtOrderDesk => _orderQueue.Count > 0;
    public bool HasCustomersAtPayDesk => _payQueue.Count > 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of `QueueController` found in scene!");
            return;
        }
        else
        {
            Instance = this;
        }

        _orderQueue = new();
        _payQueue = new();

        SpawnCustomer();
    }

    private void Start()
    {
        _game = GameManager.Instance;

#if UNITY_EDITOR
        ControlsManager.PlayerActions.Jump.performed += _ =>
        {
            SpawnCustomer();
        };
#endif
    }

    [ContextMenu("Handle Order Placed")]
    public async void HandleOrderPlaced()
    {
        if (_orderQueue.Count == 0)
        {
            Debug.LogWarning("Tried to handle order placed when order queue is empty!");
            return;
        }

        var customer = _orderQueue.Dequeue();
        customer.GenerateOrder();

        await customer.ShowOrderEmote();

        OnOrderQueueUpdated.Invoke();

        customer.Movement.OnArrived.AddListener(NotifyPayQueueUpdated);
        customer.Movement.MoveTo(_payPosition);

        _payQueue.Enqueue(customer);

        HandleQueueCustomerMoved(_orderQueue);
    }

    [ContextMenu("Handle Order Paid")]
    public async Task HandleOrderPaid()
    {
        if (_payQueue.Count == 0)
        {
            Debug.LogWarning("Tried to handle order placed when pay queue is empty!");
            return;
        }

        var customer = _payQueue.Dequeue();

        await _game.ProcessCustomerServed(customer);

        customer.Movement.MoveTo(_exitPosition);
        HandleQueueCustomerMoved(_payQueue);
    }

    [ContextMenu("Spawn Customer")]
    private void SpawnCustomer()
    {
        var randomIndex = Random.Range(0, _customerPrefabs.Length);
        var customerPrefab = _customerPrefabs[randomIndex];
        var destination = AdjustXPositionForSpacing(_orderPosition, _orderQueue.Count);

        var customer = Instantiate(customerPrefab, _spawnPosition, Quaternion.identity);
        customer.Init();
        customer.Movement.OnArrived.AddListener(NotifyOrderQueueUpdated);
        customer.Movement.MoveTo(destination);

        _orderQueue.Enqueue(customer);
    }

    private Vector2 AdjustXPositionForSpacing(Vector2 destination, int customerCount)
    {
        var newPos = destination;
        newPos.x += _customerGap * customerCount;

        return newPos;
    }

    private void HandleQueueCustomerMoved(Queue<CustomerController> queue)
    {
        int index = 0;

        foreach (var customer in queue)
        {
            Vector3 orderPos = AdjustXPositionForSpacing(_orderPosition, index++);
            customer.Movement.MoveTo(orderPos);
        }
    }

    private void NotifyOrderQueueUpdated(CustomerController customer)
    {
        customer.Movement.OnArrived.RemoveListener(NotifyOrderQueueUpdated);
        OnOrderQueueUpdated.Invoke();
    }

    private void NotifyPayQueueUpdated(CustomerController customer)
    {
        customer.Movement.OnArrived.RemoveListener(NotifyPayQueueUpdated);
        OnPayQueueUpdated.Invoke();
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
