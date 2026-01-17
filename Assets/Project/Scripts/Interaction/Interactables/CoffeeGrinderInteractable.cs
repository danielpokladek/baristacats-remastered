using PrimeTween;
using UnityEngine;

public class CoffeeGrinderInteractable : Interactable
{
    [SerializeField]
    private GameObject _beanCanvas;

    [SerializeField]
    private float _grindDuration = 2f;

    private float _grindTimer = 0;

    private bool _isGrinding = false;
    private bool _hasBeans = false;

    private Sequence _beansSequence;

    private void Start()
    {
        _beanCanvas.SetActive(false);

        _beansSequence = Sequence
            .Create(-1, Sequence.SequenceCycleMode.Yoyo, Ease.InOutSine)
            .Group(
                Tween.PositionY(
                    _beanCanvas.transform,
                    _beanCanvas.transform.position.y + 0.5f,
                    1.5f
                )
            );

        _beansSequence.isPaused = true;
    }

    private void Update()
    {
        if (!_isGrinding)
            return;

        _grindTimer += Time.deltaTime;

        if (_grindTimer >= _grindDuration)
        {
            _isGrinding = false;
            _hasBeans = true;

            _interactionPrompt.UpdateTimerFill(0);
            _interactionPrompt.ShowPrompt();

            _beanCanvas.SetActive(true);

            _beansSequence.progressTotal = 0;
            _beansSequence.isPaused = false;

            IsBusy = false;

            Debug.Log("Grind completed!");
            return;
        }

        _interactionPrompt.UpdateTimerFill(1 - (_grindTimer / _grindDuration));
    }

    public override void Interact()
    {
        if (_hasBeans)
        {
            _hasBeans = false;
            _beanCanvas.SetActive(false);

            return;
        }

        base.Interact();

        IsBusy = true;

        _grindTimer = 0;
        _isGrinding = true;

        Debug.Log("Interacted with coffee grinder.");
    }
}
