using UnityEngine;

public class RubbishBinInteractable : Interactable
{
    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        if (!player.Inventory.IsHoldingItem)
            return;

        player.Inventory.DiscardItem();
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }
}
