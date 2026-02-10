#nullable enable

using UnityEngine.Events;

public class PlayerInventory
{
    public UnityEvent OnInventoryUpdate = new();

    private MilkType _milkInHand = MilkType.NONE;
    private CoffeeData? _coffeeInHand = null;
    private bool _isHoldingBeans = false;
    private bool _isHoldingItem = false;

    public bool IsHoldingItem
    {
        get { return _isHoldingItem; }
        private set
        {
            _isHoldingItem = value;
            OnInventoryUpdate.Invoke();
        }
    }

    public MilkType MilkInHand
    {
        get { return _milkInHand; }
        set
        {
            _milkInHand = value;
            UpdateState();
        }
    }

    public CoffeeData? CoffeeInHand
    {
        get { return _coffeeInHand; }
        set
        {
            _coffeeInHand = value;
            UpdateState();
        }
    }

    public bool IsHoldingBeans
    {
        get { return _isHoldingBeans; }
        set
        {
            _isHoldingBeans = value;
            UpdateState();
        }
    }

    private void UpdateState()
    {
        bool isHoldingItem = false;

        if (_isHoldingBeans)
            isHoldingItem = true;

        if (_coffeeInHand != null)
            isHoldingItem = true;

        if (_milkInHand != MilkType.NONE)
            isHoldingItem = true;

        IsHoldingItem = isHoldingItem;
    }
}
