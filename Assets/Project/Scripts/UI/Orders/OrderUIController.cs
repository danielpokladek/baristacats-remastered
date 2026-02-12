using PrimeTween;
using UnityEngine;

public class OrderUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private OrderTicketUI _orderTicketPrefab;

    [Header("Positioning")]
    [SerializeField]
    private float _ticketXSpacing;

    public OrderTicketUI GetNewTicket()
    {
        var instance = Instantiate(_orderTicketPrefab, transform);

        // TODO: Pool these instead of instantiating new one each time.
        var currentPos = instance.transform.localPosition;
        instance.transform.localPosition = new Vector3(
            Screen.width * 2,
            currentPos.y,
            currentPos.z
        );

        return instance;
    }

    public Tween? MoveTicketToPosition(OrderTicketUI ticket, int position)
    {
        var xPos = _ticketXSpacing * position;
        var currentPos = ticket.transform.localPosition;
        var destination = new Vector3(xPos, currentPos.y, currentPos.z);

        if (ticket.transform.localPosition == destination)
            return null;

        return Tween.LocalPosition(ticket.transform, destination, duration: 0.3f);
    }

    public void DiscardTicket(OrderTicketUI ticket)
    {
        ticket.gameObject.SetActive(false);
    }
}
