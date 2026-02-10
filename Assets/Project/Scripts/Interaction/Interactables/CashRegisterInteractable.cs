public class CashRegisterInteractable : Interactable
{
    private QueueManager _queueManager;

    protected override void Start()
    {
        base.Start();

        _queueManager = QueueManager.Instance;

        CanInteract = false;

        _queueManager.OnPayQueueUpdated.AddListener(HandleInteractStateChange);
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        var inventory = player.Inventory;

        if (!inventory.IsHoldingItem || inventory.CoffeeInHand == null)
            return;

        inventory.CoffeeInHand = null;

#pragma warning disable CS4014
        // Disabling the warning here, as we don't need to await the animation.
        _queueManager.HandleOrderPaid(player.Inventory.CoffeeInHand);
#pragma warning restore
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }

    private void HandleInteractStateChange()
    {
        if (_queueManager.HasCustomersAtPayDesk)
        {
            CanInteract = true;
        }
        else
        {
            CanInteract = false;
        }
    }
}
