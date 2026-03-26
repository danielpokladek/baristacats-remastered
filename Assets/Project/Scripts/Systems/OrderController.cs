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
    GameManager _gameManager;

    [SerializeField]
    private QueueController _queue;

    [SerializeField]
    private OrderUIController _orderUI;

    [SerializeField]
    AudioSource _counterDing;

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

        Events.CustomerEvents.OnOutOfTime.AddListener(
            (customer) => _ = HandleCustomerQuit(customer)
        );

        Events.RushStart.AddListener(() =>
        {
            _spawnTimer = _spawnInterval;
        });

        Events.OnGameStart.AddListener(() =>
        {
            _spawnInterval = GetSpawnInterval();
            _spawnTimer = _spawnInterval;

            enabled = true;
        });
        Events.OnGameOver.AddListener(() =>
        {
            enabled = false;
        });

        enabled = false;
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
        return TotalOrderCount >= MaxOrders();
    }

    public int MaxOrders()
    {
        var diff = _appManager.CurrentDifficulty;
        var maxOrders = _rushController.IsRushActive ? diff.MaxOrdersRush : diff.MaxOrdersRegular;

        return maxOrders;
    }

    [ContextMenu("New Customer")]
    public async Task CreateNewCustomer()
    {
        var coffeeData = GenerateOrder();
        var customer = _queue.GetCustomer();
        var ticket = _orderUI.GetNewTicket();

        var isRush = _rushController.IsRushActive;
        var maxWaitTime = _difficultySettings.CustomerPatience.GetValue(
            isRush ? 1 : _rushController.ProgressToRush
        );

        var orderData = new OrderData
        {
            Owner = customer,
            Ticket = ticket,
            CoffeeData = coffeeData,
            MaxWaitTime = maxWaitTime,
        };

        _orderQueue.Add(orderData);

        ticket.Init(orderData);

        customer.SetOrderData(coffeeData);
        customer.Initialize(orderData);

        await _queue.MoveToOrderDesk(customer, _orderQueue.Count - 1);

        _counterDing.Play();
        _ = _orderUI.MoveTicketToPosition(ticket, TotalTicketCount - 1);

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

        var order = _payQueue.First((o) => o.CoffeeData.Milk == servedCoffee.Milk);
        order ??= _payQueue[0];

        var indexInQueue = _payQueue.IndexOf(order);
        _payQueue.RemoveAt(indexInQueue);

        _ = _orderUI.DiscardTicket(order.Ticket);

        bool wasSuccessful = EvaluateOrder(order.Owner, servedCoffee);

        if (wasSuccessful)
            Events.CustomerEvents.OnOrderSuccessful.Invoke(order.Owner);
        else
            Events.CustomerEvents.OnOrderFailed.Invoke(order.Owner);

        order.Owner.StopTimer();
        _ = order.Owner.ShowEmote(wasSuccessful);

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

        _ = customer.ShowEmote(false);

        var queue = isInOrderQueue ? _orderQueue : _payQueue;
        var otherQueue = isInOrderQueue ? _payQueue : _orderQueue;

        var order = queue.First((o) => o.Owner == customer);
        queue.Remove(order);

        _ = _orderUI.DiscardTicket(order.Ticket);
        _ = _queue.MoveToSpawn(order.Owner);

        int totalIndex = 0;

        ProcessQueue(_payQueue, _queue.MoveToPayDesk, ref totalIndex);
        ProcessQueue(_orderQueue, _queue.MoveToOrderDesk, ref totalIndex);
    }

    private CoffeeData GenerateOrder()
    {
        var maxComplexity = GetMaxOrderComplexity();
        return BuildOrderBasedOnComplexity(maxComplexity);
    }

    private CoffeeData BuildOrderBasedOnComplexity(int complexity)
    {
        var coffeeData = new CoffeeData { Quality = 80 };
        var shouldAddMilk = UnityEngine.Random.Range(0f, 1f);

        if (complexity >= 1 && shouldAddMilk >= 0.5f)
        {
            var availableMilk = GetAvailableMilk();
            var maxMilkIndex = Math.Min(complexity, availableMilk.Count());
            var milkIndex = UnityEngine.Random.Range(0, maxMilkIndex);

            var milk = availableMilk[milkIndex];
            coffeeData.Milk = milk;
        }

        return coffeeData;
    }

    private List<MilkType> GetAvailableMilk()
    {
        int unlockedCount = 1 + Mathf.FloorToInt(_gameManager.DrinksCompleted);
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

        if (servedCoffee.Milk != desiredCoffee.Milk)
            wasSuccessful = false;

        if (servedCoffee.Quality < desiredCoffee.Quality)
            wasSuccessful = false;

        return wasSuccessful;
    }

    private int GetMaxOrderComplexity()
    {
        var drinksComplexity =
            _difficultySettings.BaseDrinkComplexity
            + Mathf.FloorToInt(
                _gameManager.DrinksCompleted / _difficultySettings.DrinkComplexityScaling
            );

        return drinksComplexity;
    }

    private float GetSpawnInterval()
    {
        if (_rushController.IsRushActive)
        {
            return _difficultySettings.CustomerSpawnInterval.Min;
        }

        return _difficultySettings.CustomerSpawnInterval.GetValue(_rushController.ProgressToRush);
    }
}
