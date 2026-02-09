#nullable enable

using UnityEngine.Events;

public class PlayerInventory
{
    public UnityEvent OnInventoryUpdate = new();

    private MilkType _milkInHand = MilkType.NONE;
    private CoffeeData? _coffeeInHand = null;
    private bool _isHoldingBeans = false;

    public bool IsHoldingItem { get; private set; }

    public MilkType MilkInHand
    {
        get { return _milkInHand; }
        set
        {
            _milkInHand = value;

            if (value == MilkType.NONE)
            {
                IsHoldingItem = false;
            }
            else
            {
                IsHoldingItem = true;
            }

            OnInventoryUpdate.Invoke();
        }
    }

    public CoffeeData? CoffeeInHand
    {
        get { return _coffeeInHand; }
        set
        {
            _coffeeInHand = value;

            if (value == null)
            {
                IsHoldingItem = false;
            }
            else
            {
                IsHoldingItem = true;
            }

            OnInventoryUpdate.Invoke();
        }
    }

    public bool IsHoldingBeans
    {
        get { return _isHoldingBeans; }
        set
        {
            _isHoldingBeans = value;

            if (!value)
            {
                IsHoldingItem = false;
            }
            else
            {
                IsHoldingItem = true;
            }

            OnInventoryUpdate.Invoke();
        }
    }
}
