using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    protected InteractionPrompt _interactionPrompt;

    public bool IsBusy { get; set; }

    public virtual void Interact()
    {
        _interactionPrompt.HidePrompt();
    }

    public void ShowInteractionPrompt() => _interactionPrompt.ShowBoth();

    public void HideInteractionPrompt() => _interactionPrompt.HideBoth();

    public void UpdateInteractionTimerFill(float fillValue)
    {
        _interactionPrompt.UpdateTimerFill(fillValue);
    }
}
