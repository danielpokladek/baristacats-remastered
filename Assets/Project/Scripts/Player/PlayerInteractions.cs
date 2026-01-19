using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractions : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerController;

    private Interactable _currentInteractable;
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

            if (!_currentInteractable.CanInteract)
                return;

            _interactionDuration = 0.4f;
            _interactionTimer = 0;
            _currentInteractable.UpdateInteractionTimerFill(0);

            _isInteracting = true;
        };

        _playerActions.Interact.canceled += _ =>
        {
            if (!_currentInteractable.CanInteract)
                return;

            _isInteracting = false;
            _interactionDuration = 0;
            _interactionTimer = 0;

            _currentInteractable.UpdateInteractionTimerFill(0);
        };

        _playerActions.Interact.performed += _ =>
        {
            if (!_currentInteractable.CanInteract)
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
        if (!_isInteracting)
            return;

        _interactionTimer += Time.deltaTime;
        _currentInteractable.UpdateInteractionTimerFill(_interactionTimer / _interactionDuration);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable"))
            return;

        collision.gameObject.TryGetComponent<Interactable>(out Interactable interactable);

        if (!interactable)
        {
            Debug.LogWarning(
                "Interacted with object tagged as 'interactable', but no interactable script found!"
            );

            return;
        }

        _currentInteractable = interactable;
        _currentInteractable.PlayerInRange = true;
        _currentInteractable.ShowInteractionPrompt();

        _playerActions.Interact.Enable();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable"))
            return;

        _playerActions.Interact.Disable();

        _currentInteractable.HideInteractionPrompt();
        _currentInteractable.PlayerInRange = false;
        _currentInteractable = null;
    }
}
