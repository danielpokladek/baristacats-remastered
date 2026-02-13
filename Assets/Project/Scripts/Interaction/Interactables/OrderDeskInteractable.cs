using UnityEngine;

public class OrderDeskInteractable : Interactable
{
    [SerializeField]
    private OrderController _orderController;

    protected override void Start()
    {
        base.Start();

        CanInteract = false;

        _orderController.OnStateChange.AddListener(HandleInteractStateChange);
    }

    public override InteractionTypeEnum GetNextInteractionType()
    {
        return InteractionTypeEnum.INTERACT;
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        if (!_orderController.HasCustomersAtOrderDesk)
        {
            _interactionPrompt.ShowPrompt();
            return;
        }

        _ = _orderController.HandleOrderAccepted();
    }

    private void HandleInteractStateChange()
    {
        if (_orderController.HasCustomersAtOrderDesk)
        {
            CanInteract = true;
        }
        else
        {
            CanInteract = false;
        }
    }
}
