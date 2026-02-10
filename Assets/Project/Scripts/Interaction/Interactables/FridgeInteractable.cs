public class FridgeInteractable : Interactable
{
    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        if (player.Inventory.IsHoldingItem)
            return;

        MiniGameManager.Instance.StartMiniGame(MiniGameEnum.MILK_PICKING);
    }
}
