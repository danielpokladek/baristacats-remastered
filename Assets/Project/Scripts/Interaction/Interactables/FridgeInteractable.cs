using UnityEngine;

public class FridgeInteractable : Interactable
{
    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }
}
