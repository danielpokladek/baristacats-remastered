using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    protected InteractionPrompt _interactionPrompt;

    public abstract void Interact();

    public void ShowInteractionPrompt() => _interactionPrompt.Show();

    public void HideInteractionPrompt() => _interactionPrompt.Hide();

    public void UpdateInteractionTimerFill(float fillValue)
    {
        _interactionPrompt.UpdateTimerFill(fillValue);
    }
}
