#nullable enable

using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachineInteractable : Interactable
{
    [SerializeField]
    private CoffeeMachineSlot[] _slots;

    [SerializeField]
    private float _brewDuration = 3f;

    private int _brewingCount;

    private bool _isBrewing;

    private void Start()
    {
        foreach (var slot in _slots)
        {
            slot.BrewDuration = _brewDuration;
        }

        _brewingCount = 0;
    }

    public override void Interact()
    {
        base.Interact();

        CoffeeMachineSlot? emptySlot = GetEmptySlot();

        // TODO DP: Handle this better.
        if (!emptySlot)
            return;

        emptySlot.StartBrewing();

        _brewingCount += 1;
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
}
