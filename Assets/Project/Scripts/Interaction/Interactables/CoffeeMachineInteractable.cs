#nullable enable

using PrimeTween;
using UnityEngine;

public class CoffeeMachineInteractable : Interactable
{
    [SerializeField]
    private CoffeeMachineSlot[] _slots;

    [SerializeField]
    private CanvasGroup _noBeansCanvasGroup;

    [SerializeField]
    private float _brewDuration = 3f;

    protected override void Start()
    {
        base.Start();

        _noBeansCanvasGroup.alpha = 0;

        foreach (var slot in _slots)
        {
            slot.BrewDuration = _brewDuration;
        }
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        if (!player.Inventory.IsHoldingBeans)
        {
            ShowNoBeans();
            return;
        }

        CoffeeMachineSlot? emptySlot = GetEmptySlot();

        // TODO DP: Handle this better.
        if (!emptySlot)
            return;

        emptySlot.StartBrewing();

        player.Inventory.IsHoldingBeans = false;
    }

    private CoffeeMachineSlot? GetEmptySlot()
    {
        foreach (CoffeeMachineSlot slot in _slots)
        {
            if (slot.IsFree)
                return slot;
        }

        return null;
    }

    private void ShowNoBeans()
    {
        CanInteract = false;

        Sequence
            .Create()
            .ChainCallback(() => _interactionPrompt.HideBoth())
            .Chain(Tween.Alpha(_noBeansCanvasGroup, 1, 0.5f))
            .Chain(Tween.Delay(1.5f))
            .Chain(Tween.Alpha(_noBeansCanvasGroup, 0, 0.5f))
            .ChainCallback(() =>
            {
                if (!PlayerInRange)
                    return;

                _interactionPrompt.UpdateTimerFill(0);
                _interactionPrompt.ShowBoth();
            });

        CanInteract = true;
    }
}
