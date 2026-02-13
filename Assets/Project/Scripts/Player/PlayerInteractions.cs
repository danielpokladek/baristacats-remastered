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

    private void Start()
    {
        _playerActions = ControlsManager.PlayerActions;
        _playerActions.Interact.Disable();

        _playerActions.Interact.performed += _ =>
        {
            if (!_currentInteractable || !_currentInteractable.CanInteract)
                return;

            _currentInteractable.Interact(_playerController);
        };
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
