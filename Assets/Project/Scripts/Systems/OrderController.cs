using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private QueueController _queue;

    [SerializeField]
    private OrderUIController _orderUI;

    private Queue<OrderData> _orderQueue = new();
    private Queue<OrderData> _payQueue = new();

    private MilkType[] _milkTypeValues;

    public UnityEvent OnStateChange = new();

    public bool HasCustomersAtOrderDesk => _orderQueue.Count > 0;
    public bool HasCustomersAtPayDesk => _payQueue.Count > 0;
    public int TotalTicketCount => _orderQueue.Count + _payQueue.Count;

    private void Start()
    {
        _milkTypeValues = (MilkType[])Enum.GetValues(typeof(MilkType));
    }

    [ContextMenu("New Customer")]
    public async Task CreateNewCustomer()
    {
        var coffeeData = GenerateOrder();
        var customer = _queue.GetCustomer();
        var ticket = _orderUI.GetNewTicket();

        var orderData = new OrderData
        {
            Owner = customer,
            Ticket = ticket,
            CoffeeData = coffeeData,
            MaxWaitTime = UnityEngine.Random.Range(10, 30),
        };

        _orderQueue.Enqueue(orderData);

        customer.SetOrderData(coffeeData);
        ticket.Init(orderData);

        _ = _orderUI.MoveTicketToNextSpot(ticket, TotalTicketCount - 1);
        await _queue.MoveToOrderDesk(customer, _orderQueue.Count - 1);

        ticket.StartTimer();

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

        var order = _orderQueue.Dequeue();
        _payQueue.Enqueue(order);

        order.Ticket.RevealOrder();

        await _queue.MoveToPayTable(order.Owner, _payQueue.Count - 1);

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

        var order = _payQueue.Dequeue();

        // TODO: Add animation to ticket moving out of view.
        order.Ticket.gameObject.SetActive(false);
        _queue.MoveToExit(order.Owner);
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
}
