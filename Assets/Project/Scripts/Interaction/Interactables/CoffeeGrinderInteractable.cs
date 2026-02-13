using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeGrinderInteractable : Interactable
{
    [Header("References")]
    [SerializeField]
    private Image _beanImage;

    [SerializeField]
    private Image _timerImage;

    [Header("Grinder Settings")]
    [SerializeField]
    private int _grindsRequired = 10;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _grinderSound;

    private int _grindsComplete = 0;

    private bool _isGrinding = false;
    private bool _hasBeans = false;

    private AudioSource _audioSource;

    private Sequence _beanIconSequence;

    protected override void Start()
    {
        base.Start();

        _audioSource = GetComponent<AudioSource>();

        _beanIconSequence = GetItemReadySequence(_beanImage.transform);
        _beanIconSequence.isPaused = true;

        _beanImage.gameObject.SetActive(false);
        _timerImage.gameObject.SetActive(false);
    }

    // private void Update()
    // {
    //     if (!_isGrinding)
    //         return;

    //     _grindTimer += Time.deltaTime;

    //     if (_grindTimer >= _grindDuration)
    //     {
    //         CompleteGrind();
    //         return;
    //     }

    //     _timerImage.fillAmount = 1 - (_grindTimer / _grindDuration);
    // }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        if (_hasBeans)
            return InteractionTypeEnum.COLLECT;

        return InteractionTypeEnum.INTERACT;
    }

    public override void Interact(PlayerController player)
    {
        var playerHasItem = player.Inventory.IsHoldingItem;
        var grinderHasBeans = _hasBeans;

        if (playerHasItem && grinderHasBeans)
            return;

        base.Interact(player);

        if (!grinderHasBeans && playerHasItem)
        {
            StartGrinding();
            return;
        }

        if (grinderHasBeans && !playerHasItem)
        {
            _hasBeans = false;
            _beanImage.gameObject.SetActive(false);

            player.Inventory.IsHoldingBeans = true;

            return;
        }

        if (!_isGrinding)
        {
            StartGrinding();
        }

        HandleBeansGrind();
    }

    private void HandleBeansGrind()
    {
        _audioSource.PlayOneShot(_grinderSound);

        _grindsComplete += 1;
        _timerImage.fillAmount = (float)_grindsComplete / _grindsRequired;

        if (_grindsComplete >= _grindsRequired)
        {
            CompleteGrind();
        }
    }

    private void StartGrinding()
    {
        if (_isGrinding)
            return;

        _timerImage.fillAmount = 1;
        _timerImage.gameObject.SetActive(true);

        _grindsComplete = 0;
        _isGrinding = true;
    }

    private void CompleteGrind()
    {
        _timerImage.gameObject.SetActive(false);
        _timerImage.fillAmount = 1;

        _isGrinding = false;
        _hasBeans = true;

        if (PlayerInRange)
        {
            _interactionPrompt?.ShowPrompt();
        }

        _beanImage.gameObject.SetActive(true);
        _beanIconSequence.isPaused = false;
    }
}
