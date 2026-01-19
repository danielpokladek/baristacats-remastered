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

    public void ShowInteractionPrompt() => _interactionPrompt.ShowBoth();

    public void HideInteractionPrompt() => _interactionPrompt.HideBoth();

    public void UpdateInteractionTimerFill(float fillValue)
    {
        _interactionPrompt.UpdateTimerFill(fillValue);
    }
}
