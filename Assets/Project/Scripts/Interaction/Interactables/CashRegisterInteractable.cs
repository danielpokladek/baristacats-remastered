using UnityEngine;

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

        _queueManager.HandleOrderPaid();
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
