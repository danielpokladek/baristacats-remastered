using UnityEngine;

public class OrderDeskInteractable : Interactable
{
    [SerializeField]
    private QueueManager _queueManager;

    protected override void Start()
    {
        base.Start();

        CanInteract = false;

        _queueManager.OnOrderQueueUpdated.AddListener(HandleInteractStateChange);
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        if (!_queueManager.HasCustomersAtOrderDesk)
        {
            _interactionPrompt.ShowBoth();
            return;
        }

        _queueManager.HandleOrderPlaced();
    }

    private void HandleInteractStateChange()
    {
        if (_queueManager.HasCustomersAtOrderDesk)
        {
            CanInteract = true;
        }
        else
        {
            CanInteract = false;
        }
    }
}
