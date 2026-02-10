using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeTween;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
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

    [Header("Audio")]
    [SerializeField]
    private AudioClip _doorSound;

    private Queue<CustomerController> _orderQueue;
    private Queue<CustomerController> _payQueue;

    private GameManager _game;
    private AudioSource _audioSource;

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
    }

    private void Start()
    {
        _game = GameManager.Instance;
        _audioSource = GetComponent<AudioSource>();

#if UNITY_EDITOR
        ControlsManager.PlayerActions.Jump.performed += _ =>
        {
#pragma warning disable CS4014
            // Disabling the warning here, as we don't need to await the animation.
            SpawnCustomer();
        };
#endif

        // Disabling the warning here, as we don't need to await the animation.
        SpawnCustomer();
#pragma warning restore
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
        var pos = GetPositionWithSpacing(_payPosition, _payQueue.Count);

        _payQueue.Enqueue(customer);

        var coffeeData = GenerateOrder();
        customer.SetCoffeeOrder(coffeeData);

        await customer.ShowOrderEmote();

        OnOrderQueueUpdated.Invoke();

        customer.Movement.OnArrived.AddListener(NotifyPayQueueUpdated);
        customer.Movement.MoveTo(pos);

        HandleQueueCustomerMoved(_orderQueue);
    }

    [ContextMenu("Handle Order Paid")]
    public async Task HandleOrderPaid(CoffeeData servedCoffee)
    {
        if (_payQueue.Count == 0)
        {
            Debug.LogWarning("Tried to handle order placed when pay queue is empty!");
            return;
        }

        var customer = _payQueue.Dequeue();
        customer.ServedCoffee.Milk = servedCoffee.Milk;
        customer.ServedCoffee.Quality = servedCoffee.Quality;

        await _game.ProcessCustomerServed(customer);

        customer.Movement.MoveTo(_exitPosition);
        HandleQueueCustomerMoved(_payQueue);
    }

    [ContextMenu("Spawn Customer")]
    private async Task SpawnCustomer()
    {
        var randomIndex = Random.Range(0, _customerPrefabs.Length);
        var customerPrefab = _customerPrefabs[randomIndex];
        var destination = GetPositionWithSpacing(_orderPosition, _orderQueue.Count);

        var customer = Instantiate(customerPrefab, _spawnPosition, Quaternion.identity);
        customer.Init();

        _orderQueue.Enqueue(customer);

        _audioSource.PlayOneShot(_doorSound);

        await Tween.Delay(5.2f);

        customer.Movement.OnArrived.AddListener(NotifyOrderQueueUpdated);
        customer.Movement.MoveTo(destination);
    }

    private Vector2 GetPositionWithSpacing(Vector2 destination, int customerCount)
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
            Vector3 orderPos = GetPositionWithSpacing(_orderPosition, index++);
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

    private CoffeeData GenerateOrder()
    {
        var coffeeData = new CoffeeData();

        var randomIndex = Random.Range(0, 3);
        var milkType = MilkType.NONE;

        // TODO: Add some randomization to this.
        var minQuality = 80;

        switch (randomIndex)
        {
            case 0:
                break;

            case 1:
                milkType = MilkType.BLUE;
                break;

            case 2:
                milkType = MilkType.RED;
                break;

            case 3:
                milkType = MilkType.YELLOW;
                break;

            case 4:
                milkType = MilkType.CYAN;
                break;
        }

        coffeeData.Milk = milkType;
        coffeeData.Quality = minQuality;

        return coffeeData;
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
