using PrimeTween;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    protected InteractionPrompt _interactionPrompt;

    public bool CanInteract { get; set; }

    public bool PlayerInRange { get; set; }

    protected virtual void Start()
    {
        CanInteract = true;
        PlayerInRange = false;
    }

    public virtual void Interact(PlayerController player)
    {
        _interactionPrompt.HidePrompt();
    }

    public abstract InteractionTypeEnum GetNextInteractionType();

    public void ShowInteractionPrompt()
    {
        if (!CanInteract)
            return;

        _interactionPrompt.ShowBoth();
    }

    public void HideInteractionPrompt() => _interactionPrompt.HideBoth();

    public void UpdateInteractionTimerFill(float fillValue)
    {
        _interactionPrompt.UpdateTimerFill(fillValue);
    }

    protected Sequence GetItemReadySequence(Transform item)
    {
        return Sequence
            .Create(-1, Sequence.SequenceCycleMode.Yoyo, Ease.InOutSine)
            .Group(Tween.PositionY(item.transform, item.transform.position.y + 0.5f, 1.5f));
    }
}
