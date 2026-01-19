using UnityEngine;

public class CashRegisterInteractable : Interactable
{
    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        var inventory = player.Inventory;

        if (!inventory.IsHoldingItem || inventory.CoffeeInHand == null)
            return;

        Debug.Log($"Coffee served! Quality: {inventory.CoffeeInHand.Quality}");
        inventory.CoffeeInHand = null;
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }
}
