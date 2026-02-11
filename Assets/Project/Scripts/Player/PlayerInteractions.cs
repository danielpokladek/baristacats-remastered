#nullable enable

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractions : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerController = null!;

    private readonly HashSet<Interactable> _interactablesInRange = new();
    private Interactable? _currentInteractable;

    private InputSystem_Actions.PlayerActions _playerActions;

    private bool _isInteracting = false;

    private float _interactionDuration = 0;
    private float _interactionTimer = 0;

    private void Start()
    {
        _playerActions = ControlsManager.PlayerActions;
        _playerActions.Interact.Disable();

        _playerActions.Interact.started += ctx =>
        {
            // TODO DP: For some reason this causes `infinity`.
            // var holdInteraction = ctx.interaction as HoldInteraction;

            if (!_currentInteractable || !_currentInteractable.CanInteract)
                return;

            _interactionDuration = 0.4f;
            _interactionTimer = 0;
            _currentInteractable.UpdateInteractionTimerFill(0);

            _isInteracting = true;
        };

        _playerActions.Interact.canceled += _ =>
        {
            if (!_currentInteractable || !_currentInteractable.CanInteract)
                return;

            _isInteracting = false;
            _interactionDuration = 0;
            _interactionTimer = 0;

            _currentInteractable.UpdateInteractionTimerFill(0);
        };

        _playerActions.Interact.performed += _ =>
        {
            if (!_currentInteractable || !_currentInteractable.CanInteract)
                return;

            _isInteracting = false;
            _interactionDuration = 0;
            _interactionTimer = 0;

            _currentInteractable.UpdateInteractionTimerFill(0);
            _currentInteractable.Interact(_playerController);
        };
    }

    private void Update()
    {
        if (!_isInteracting || !_currentInteractable)
            return;

        _interactionTimer += Time.deltaTime;
        _currentInteractable.UpdateInteractionTimerFill(_interactionTimer / _interactionDuration);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable"))
            return;

        if (!collision.gameObject.TryGetComponent(out Interactable interactable))
        {
            Debug.LogWarning(
                "Interacted with object tagged as 'interactable', but no interactable script found!"
            );

            return;
        }

        _interactablesInRange.Add(interactable);
        UpdateCurrentInteractable();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable"))
            return;

        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        _interactablesInRange.Remove(interactable);
        UpdateCurrentInteractable();
    }

    private void UpdateCurrentInteractable()
    {
        Interactable? next = null;
        float bestDistSq = float.MaxValue;

        Vector2 playerPos = transform.position;

        foreach (var interactable in _interactablesInRange)
        {
            Vector2 interactablePos = interactable.transform.position;
            float distSq = (interactablePos - playerPos).sqrMagnitude;

            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                next = interactable;
            }
        }

        if (_currentInteractable == next)
            return;

        if (_currentInteractable != null)
        {
            _currentInteractable.HideInteractionPrompt();
            _currentInteractable.PlayerInRange = false;
        }

        _currentInteractable = next;

        if (_currentInteractable != null)
        {
            _currentInteractable.PlayerInRange = true;
            _currentInteractable.ShowInteractionPrompt();

            _playerActions.Interact.Enable();
        }
        else
        {
            _playerActions.Interact.Disable();
        }
    }
}
