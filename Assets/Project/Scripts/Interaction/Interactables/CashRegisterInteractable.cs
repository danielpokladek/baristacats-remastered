using UnityEngine;

public class CashRegisterInteractable : Interactable
{
    [SerializeField]
    private OrderController _orderController;

    protected override void Start()
    {
        base.Start();

        CanInteract = false;

        _orderController.OnStateChange.AddListener(HandleInteractStateChange);
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        var inventory = player.Inventory;

        if (!inventory.IsHoldingItem || inventory.CoffeeInHand == null)
            return;

        var servedCoffee = inventory.CoffeeInHand;
        inventory.CoffeeInHand = null;

        _ = _orderController.HandleOrderComplete(servedCoffee);
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }

    private void HandleInteractStateChange()
    {
        if (_orderController.HasCustomersAtPayDesk)
        {
            CanInteract = true;
        }
        else
        {
            CanInteract = false;
        }
    }
}
