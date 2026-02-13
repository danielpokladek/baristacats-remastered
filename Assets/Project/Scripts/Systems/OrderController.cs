using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimeTween;
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

    private float _spawnInterval;
    private float _spawnTimer;

    private ApplicationManager _appManager;
    private DifficultyController _difficultyController;
    private RushController _rushController;

    private DifficultySettingsSO _difficultySettings;

    private List<MilkType> _availableMilks = Enum.GetValues(typeof(MilkType))
        .Cast<MilkType>()
        .ToList();

    private void Start()
    {
        _appManager = ApplicationManager.Instance;

        if (!_appManager)
        {
            Debug.LogError("Unable to initialize OrderController, ApplicationManager not found!");
            return;
        }

        _rushController = _gameManager.RushController;

        _milkTypeValues = (MilkType[])Enum.GetValues(typeof(MilkType));

        _difficultySettings = ApplicationManager.Instance.CurrentDifficulty;
        _difficultyController = _gameManager.DifficultyController;

        var difficulty = _difficultyController.CurrentDifficulty;

        _spawnInterval = GetSpawnInterval();
        _spawnTimer = _spawnInterval;

        Events.CustomerEvents.RanOutOfPatience.AddListener(
            (customer) => _ = HandleCustomerQuit(customer)
        );

        Events.RushStart.AddListener(() =>
        {
            _spawnTimer = _spawnInterval;
        });
    }

    private void Update()
    {
        if (IsAtMaxOrders())
            return;

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0;
            _spawnInterval = GetSpawnInterval();

            _ = CreateNewCustomer();
        }
    }

    public int TotalOrderCount => _orderQueue.Count + _payQueue.Count;

    public bool IsAtMaxOrders()
    {
        var baseMaxOrders = _appManager.CurrentDifficulty.MaxOrdersQueued;
        var adjustedMaxOrders = _rushController.IsRushActive ? baseMaxOrders * 2 : baseMaxOrders;

        return TotalOrderCount >= adjustedMaxOrders;
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

        // TODO: Debug only, remove later.
        ticket.RevealOrder();

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

    public async Task HandleOrderComplete(CoffeeData servedCoffee)
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

        bool wasSuccessful = EvaluateOrder(order.Owner, servedCoffee);

        if (wasSuccessful)
        {
            Events.CustomerEvents.OrderSuccessful.Invoke(order.Owner);
        }
        else
        {
            Events.CustomerEvents.OrderFailed.Invoke(order.Owner);
        }

        await order.Owner.ShowEmote(wasSuccessful);

        _ = _queue.MoveToExit(order.Owner);

        int totalIndex = 0;

        ProcessQueue(_payQueue, _queue.MoveToPayDesk, ref totalIndex);
        ProcessQueue(_orderQueue, _queue.MoveToOrderDesk, ref totalIndex);
    }

    private async Task HandleCustomerQuit(CustomerController customer)
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

        await customer.ShowEmote(false);

        var queue = isInOrderQueue ? _orderQueue : _payQueue;
        var otherQueue = isInOrderQueue ? _payQueue : _orderQueue;

        var order = queue.First((o) => o.Owner == customer);
        queue.Remove(order);

        _ = _orderUI.DiscardTicket(order.Ticket);
        _ = _queue.MoveToExit(order.Owner);

        int totalIndex = 0;

        ProcessQueue(_payQueue, _queue.MoveToPayDesk, ref totalIndex);
        ProcessQueue(_orderQueue, _queue.MoveToOrderDesk, ref totalIndex);
    }

    private CoffeeData GenerateOrder()
    {
        var maxComplexity = GetMaxOrderComplexity();
        var drinkComplexity = UnityEngine.Random.Range(1f, maxComplexity);

        return BuildOrderBasedOnComplexity(drinkComplexity);
    }

    private CoffeeData BuildOrderBasedOnComplexity(float complexity)
    {
        // print($"Creating new order - drink complexity: {complexity}");

        var coffeeData = new CoffeeData { Quality = 80 };

        var availableMilk = GetAvailableMilk();

        if (complexity > 2)
        {
            var milk = availableMilk[UnityEngine.Random.Range(0, availableMilk.Count())];
            coffeeData.Milk = milk;
        }

        return coffeeData;
    }

    private List<MilkType> GetAvailableMilk()
    {
        int unlockedCount = 1 + Mathf.FloorToInt(_difficultyController.CurrentDifficulty * 1);
        unlockedCount = Mathf.Clamp(unlockedCount, 1, _milkTypeValues.Count() - 1);

        return _availableMilks.Take(unlockedCount).ToList();
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

    private bool EvaluateOrder(CustomerController customer, CoffeeData servedCoffee)
    {
        var desiredCoffee = customer.DesiredCoffee;
        var wasSuccessful = true;

        print(
            $"Desired Milk: {desiredCoffee.Milk} | Served Milk: {servedCoffee.Milk} | {desiredCoffee.Milk == servedCoffee.Milk}"
        );
        print(
            $"Desired Quality: {desiredCoffee.Quality} | Served Quality: {servedCoffee.Quality} | {servedCoffee.Quality >= desiredCoffee.Quality}"
        );

        if (servedCoffee.Milk != desiredCoffee.Milk)
        {
            wasSuccessful = false;
        }

        if (servedCoffee.Quality < desiredCoffee.Quality)
        {
            wasSuccessful = false;
        }

        return wasSuccessful;
    }

    private float GetMaxOrderComplexity()
    {
        return _difficultySettings.BaseDrinkComplexity
            + Mathf.FloorToInt(
                _difficultyController.CurrentDifficulty * _difficultySettings.DrinkComplexityScaling
            );
    }

    private float GetSpawnInterval()
    {
        var spawnInterval =
            _difficultySettings.CustomerSpawnInterval.GetRandomValue()
            / (
                1f
                + _difficultyController.CurrentDifficulty * _difficultySettings.CustomerSpawnScaling
            );

        if (_gameManager.RushController.IsRushActive)
        {
            spawnInterval *= 2;
        }

        return spawnInterval;
    }
}
