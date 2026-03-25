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

        var playerInventory = player.Inventory;
        var isAnyCoffeeReady = _brewedCoffee > 0;

        if (!playerInventory.IsHoldingItem && isAnyCoffeeReady)
        {
            RetrieveReadyCoffee(playerInventory);
            return;
        }

        CoffeeMachineSlot? freeSlot = GetFreeSlot();

        if (playerInventory.IsHoldingBeans && freeSlot != null)
        {
            StartBrewing(playerInventory, freeSlot);
            return;
        }

        CoffeeMachineSlot? brewedSlot = GetBrewedSlot();

        if (playerInventory.MilkInHand != MilkType.NONE && isAnyCoffeeReady && brewedSlot != null)
        {
            StartFrothingMinigame(playerInventory, brewedSlot);
            return;
        }

        if (!playerInventory.IsHoldingBeans)
        {
            ShowNoBeans();
            return;
        }
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

    private CoffeeMachineSlot? GetBrewedSlot()
    {
        foreach (CoffeeMachineSlot slot in _slots)
        {
            if (!slot.IsFree && !slot.IsBrewing)
                return slot;
        }

        return null;
    }

    private void RetrieveReadyCoffee(PlayerInventory inventory)
    {
        var completedSlot = GetBrewedSlot();

        if (completedSlot)
        {
            inventory.CoffeeInHand = new CoffeeData { Milk = MilkType.NONE, Quality = 100 };

            _brewedCoffee--;

            completedSlot.Reset();

            return;
        }
    }

    private void StartBrewing(PlayerInventory inventory, CoffeeMachineSlot slot)
    {
        inventory.IsHoldingBeans = false;
        _ = slot.StartBrewing();
    }

    private void StartFrothingMinigame(PlayerInventory inventory, CoffeeMachineSlot slot)
    {
        inventory.CoffeeInHand = new CoffeeData { Milk = MilkType.NONE, Quality = 100 };

        _brewedCoffee--;
        slot.Reset();

        MiniGameManager.Instance.StartFrothing();
        return;
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
