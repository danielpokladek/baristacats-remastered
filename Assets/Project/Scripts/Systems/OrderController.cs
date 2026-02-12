using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using NUnit.Framework.Constraints;
using PrimeTween;
using UnityEditor.PackageManager.Requests;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Events;

public class OrderData
{
    public CustomerController Owner;
    public OrderTicketUI Ticket;
    public CoffeeData CoffeeData;
    public float MaxWaitTime;
}

public class OrderController : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private QueueController _queue;

    [SerializeField]
    private OrderUIController _orderUI;

    private List<OrderData> _orderQueue = new();
    private List<OrderData> _payQueue = new();

    private MilkType[] _milkTypeValues;

    public UnityEvent OnStateChange = new();

    public bool HasCustomersAtOrderDesk => _orderQueue.Count > 0;
    public bool HasCustomersAtPayDesk => _payQueue.Count > 0;
    public int TotalTicketCount => _orderQueue.Count + _payQueue.Count;

    private float _spawnInterval = 0f;
    private float _spawnTimer = 0f;

    private ApplicationManager _appManager;

    private void Start()
    {
        _appManager = ApplicationManager.Instance;

        if (!_appManager)
        {
            Debug.LogError("Unable to initialize OrderController, ApplicationManager not found!");
            return;
        }

        _milkTypeValues = (MilkType[])Enum.GetValues(typeof(MilkType));

        var difficulty = _gameManager.DifficultyController.CurrentDifficulty;
        var difficultySettings = ApplicationManager.Instance.CurrentDifficulty;

        _spawnInterval =
            difficultySettings.BaseCustomerSpawnInterval
            / (1f + difficulty * difficultySettings.CustomerSpawnScaling);

        _spawnTimer = _spawnInterval;

        Events.CustomerEvents.PatienceLost.AddListener(HandleCustomerQuit);
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0;
            _ = CreateNewCustomer();
        }
    }

    private void HandleCustomerQuit(CustomerController customer)
    {
        bool isInOrderQueue = _orderQueue.Count((a) => a.Owner == customer) > 0;
        bool isInPayQueue = _payQueue.Count((a) => a.Owner == customer) > 0;

        if (!isInOrderQueue && !isInPayQueue)
        {
            Debug.LogError("Tried to remove customer, but they're not in order OR pay queue!");
            return;
        }

        if (isInOrderQueue && isInPayQueue)
        {
            Debug.LogError("Tried to remove customer, but they are in both queues!");
            return;
        }

        var queue = isInOrderQueue ? _orderQueue : _payQueue;
        var otherQueue = isInOrderQueue ? _payQueue : _orderQueue;

        var order = queue.First((o) => o.Owner == customer);

        _orderUI.DiscardTicket(order.Ticket);

        _queue.MoveToExit(order.Owner);

        int totalIndex = 0;

        ProcessQueue(_payQueue, _queue.MoveToPayDesk, ref totalIndex);
        ProcessQueue(_orderQueue, _queue.MoveToOrderDesk, ref totalIndex);
    }

    [ContextMenu("New Customer")]
    public async Task CreateNewCustomer()
    {
        var coffeeData = GenerateOrder();
        var customer = _queue.GetCustomer(_appManager, _gameManager.DifficultyController);
        var ticket = _orderUI.GetNewTicket();

        var orderData = new OrderData
        {
            Owner = customer,
            Ticket = ticket,
            CoffeeData = coffeeData,
            MaxWaitTime = UnityEngine.Random.Range(10, 30),
        };

        _orderQueue.Add(orderData);

        ticket.Init(orderData);

        customer.SetOrderData(coffeeData);
        customer.Initialize(ticket);

        _ = _orderUI.MoveTicketToPosition(ticket, TotalTicketCount - 1);
        await _queue.MoveToOrderDesk(customer, _orderQueue.Count - 1);

        orderData.Owner.StartTimer();

        OnStateChange.Invoke();
    }

    public async Task HandleOrderAccepted()
    {
        if (_orderQueue.Count == 0)
        {
            Debug.LogWarning(
                "Tried to dequeue an order at order desk, but there are no order available!"
            );
            return;
        }

        var order = _orderQueue[0];
        _orderQueue.RemoveAt(0);

        _payQueue.Add(order);

        order.Ticket.RevealOrder();

        // _queue.HandleQueueCustomerMoved(_orderQueue);
        int totalIndex = 0;

        ProcessQueue(_payQueue, _queue.MoveToPayDesk, ref totalIndex);
        ProcessQueue(_orderQueue, _queue.MoveToOrderDesk, ref totalIndex);

        await _queue.MoveToPayDesk(order.Owner, _payQueue.Count - 1);

        OnStateChange.Invoke();
    }

    public void HandleOrderComplete()
    {
        if (_payQueue.Count == 0)
        {
            Debug.LogWarning(
                "Tried to dequeue an order at pay counter, but there are no order available!"
            );
            return;
        }

        var order = _payQueue[0];
        _payQueue.RemoveAt(0);

        // TODO: Add animation to ticket moving out of view.
        order.Ticket.gameObject.SetActive(false);
        _queue.MoveToExit(order.Owner);

        // _queue.HandleQueueCustomerMoved(_payQueue);

        int totalIndex = 0;

        ProcessQueue(_payQueue, _queue.MoveToPayDesk, ref totalIndex);
        ProcessQueue(_orderQueue, _queue.MoveToOrderDesk, ref totalIndex);
    }

    private CoffeeData GenerateOrder()
    {
        var coffeeData = new CoffeeData();

        // TODO: Add some randomization to this.
        var minQuality = 80;

        coffeeData.Milk = _milkTypeValues[UnityEngine.Random.Range(0, _milkTypeValues.Length)];
        coffeeData.Quality = minQuality;

        return coffeeData;
    }

    private void ProcessQueue(
        List<OrderData> queue,
        Func<CustomerController, int, Tween> moveAction,
        ref int totalIndex
    )
    {
        int index = 0;

        foreach (var order in queue)
        {
            moveAction(order.Owner, index);
            _orderUI.MoveTicketToPosition(order.Ticket, totalIndex);

            index++;
            totalIndex++;
        }
    }
}
