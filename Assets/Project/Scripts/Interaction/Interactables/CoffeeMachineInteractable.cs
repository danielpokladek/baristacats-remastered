#nullable enable

using PrimeTween;
using UnityEngine;

public class CoffeeMachineInteractable : Interactable
{
    [SerializeField]
    private CoffeeMachineSlot[] _slots = null!;

    [SerializeField]
    private CanvasGroup _noBeansCanvasGroup = null!;

    [SerializeField]
    private float _brewDuration = 3f;

    private int _brewedCoffee = 0;

    protected override void Start()
    {
        base.Start();

        _noBeansCanvasGroup.alpha = 0;

        foreach (var slot in _slots)
        {
            slot.BrewDuration = _brewDuration;
            slot.OnCoffeeBrewed.AddListener(() =>
            {
                _brewedCoffee++;

                if (PlayerInRange)
                    ShowInteractionPrompt();
            });
        }
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        var inventory = player.Inventory;
        var isAnyCoffeeReady = _brewedCoffee > 0;

        if (!inventory.IsHoldingItem && isAnyCoffeeReady)
        {
            var completedSlot = GetReadySlot();

            if (completedSlot)
            {
                player.Inventory.CoffeeInHand = new CoffeeData
                {
                    Milk = MilkType.NONE,
                    Quality = 100,
                };

                _brewedCoffee--;

                completedSlot.Reset();

                return;
            }
        }

        CoffeeMachineSlot? freeSlot = GetFreeSlot();

        if (inventory.IsHoldingBeans && freeSlot != null)
        {
            player.Inventory.IsHoldingBeans = false;
            _ = freeSlot.StartBrewing();

            return;
        }

        if (inventory.MilkInHand != MilkType.NONE && isAnyCoffeeReady)
        {
            var completedSlot = GetReadySlot();

            if (completedSlot)
            {
                player.Inventory.CoffeeInHand = new CoffeeData
                {
                    Milk = MilkType.NONE,
                    Quality = 100,
                };

                completedSlot.Reset();
                _brewedCoffee--;
            }

            MiniGameManager.Instance.StartFrothing();
            return;
        }

        if (!inventory.IsHoldingBeans)
            ShowNoBeans();
    }

    private CoffeeMachineSlot? GetFreeSlot()
    {
        foreach (CoffeeMachineSlot slot in _slots)
        {
            if (slot.IsFree)
                return slot;
        }

        return null;
    }

    private CoffeeMachineSlot? GetReadySlot()
    {
        foreach (CoffeeMachineSlot slot in _slots)
        {
            if (!slot.IsFree && !slot.IsBrewing)
                return slot;
        }

        return null;
    }

    private void ShowNoBeans()
    {
        CanInteract = false;

        Sequence
            .Create()
            .ChainCallback(() => _interactionPrompt?.HidePrompt())
            .Chain(Tween.Alpha(_noBeansCanvasGroup, 1, 0.5f))
            .Chain(Tween.Delay(1.5f))
            .Chain(Tween.Alpha(_noBeansCanvasGroup, 0, 0.5f))
            .ChainCallback(() =>
            {
                if (!PlayerInRange)
                    return;

                _interactionPrompt?.ShowPrompt();
            });

        CanInteract = true;
    }
}
