#nullable enable

using PrimeTween;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    protected InteractionPrompt? _interactionPrompt;

    [SerializeField]
    private SpriteRenderer _outlineRenderer;

    public bool CanInteract { get; set; }

    public bool PlayerInRange { get; set; }

    protected Material? _material;

    protected virtual void Start()
    {
        CanInteract = true;
        PlayerInRange = false;

        if (_outlineRenderer)
        {
            _material = _outlineRenderer.material;
        }
    }

    public virtual void Interact(PlayerController player)
    {
        _interactionPrompt!.HidePrompt();
    }

    public abstract InteractionTypeEnum GetNextInteractionType();

    public void ShowInteractionPrompt()
    {
        if (!CanInteract)
            return;

        _interactionPrompt!.ShowBoth();
        ShowOutline();
    }

    public void HideInteractionPrompt()
    {
        _interactionPrompt!.HideBoth();
        HideOutline();
    }

    public void UpdateInteractionTimerFill(float fillValue)
    {
        _interactionPrompt!.UpdateTimerFill(fillValue);
    }

    protected Sequence GetItemReadySequence(Transform item)
    {
        return Sequence
            .Create(-1, Sequence.SequenceCycleMode.Yoyo, Ease.InOutSine)
            .Group(Tween.PositionY(item.transform, item.transform.position.y + 0.5f, 1.5f));
    }

    protected void ShowOutline()
    {
        if (!_material)
            return;

        Tween.Custom(
            0f,
            1f,
            duration: 0.15f,
            onValueChange: val => _material.SetFloat("_Strength", val)
        );
    }

    protected void HideOutline()
    {
        if (!_material)
            return;

        Tween.Custom(
            1f,
            0f,
            duration: 0.15f,
            onValueChange: val => _material.SetFloat("_Strength", val)
        );
    }
}
