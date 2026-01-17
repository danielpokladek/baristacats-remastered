using UnityEngine;

public class CoffeeGrinderInteractable : Interactable
{
    [SerializeField]
    private float _grindDuration = 2f;

    private float _grindTimer = 0;

    private bool _isGrinding = false;

    private void Update()
    {
        if (!_isGrinding)
            return;

        _grindTimer += Time.deltaTime;

        if (_grindTimer >= _grindDuration)
        {
            _isGrinding = false;

            _interactionPrompt.UpdateTimerFill(0);
            _interactionPrompt.ShowPrompt();

            IsBusy = false;

            Debug.Log("Grind completed!");
            return;
        }

        _interactionPrompt.UpdateTimerFill(1 - (_grindTimer / _grindDuration));
    }

    public override void Interact()
    {
        base.Interact();

        IsBusy = true;

        _grindTimer = 0;
        _isGrinding = true;

        Debug.Log("Interacted with coffee grinder.");
    }
}
