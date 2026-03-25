using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CustomerMovement))]
public class CustomerController : MonoBehaviour
{
    [SerializeField]
    GameManager _gameManager;

    [SerializeField]
    Image _emoteImage;

    [SerializeField]
    Image _coffeeImage;

    public CoffeeData DesiredCoffee { get; private set; }

    public CustomerMovement Movement { get; private set; }

    private CharacterEmotes _emotes;
    private OrderTicketUI _ticket;

    private float _waitTime = 0f;

    private Tween? _timerTween;

    public void Setup()
    {
        _emotes = CharacterEmotes.Instance;

        Movement = GetComponent<CustomerMovement>();

        _emoteImage.color = new(1, 1, 1, 0);
        _coffeeImage.color = new(1, 1, 1, 0);

        DesiredCoffee = new();
    }

    public void Initialize(OrderData orderData)
    {
        _ticket = orderData.Ticket;
        _waitTime = orderData.MaxWaitTime;
    }

    public void StartTimer()
    {
        _timerTween = Tween
            .Custom(_waitTime, 0, _waitTime, (val) => _ticket.SetPatienceBarFill(val / _waitTime))
            .OnComplete(() => Events.CustomerEvents.OnOutOfTime.Invoke(this));
    }

    public void StopTimer()
    {
        _timerTween?.Stop();
    }

    public void SetOrderData(CoffeeData coffeeData)
    {
        DesiredCoffee.Milk = coffeeData.Milk;
        DesiredCoffee.Quality = coffeeData.Quality;
    }

    public async Task ShowOrderEmote()
    {
        _emoteImage.sprite = CharacterEmotes.Instance.PlainEmote;
        _coffeeImage.sprite = CharacterEmotes.Instance.PlainCoffeeEmote;

        await Sequence
            .Create()
            .Group(Tween.Alpha(_emoteImage, 1f, 0.5f))
            .Group(Tween.Alpha(_coffeeImage, 1f, 0.5f));

        await Tween.Delay(1.5f);

        await Sequence
            .Create()
            .Group(Tween.Alpha(_emoteImage, 0f, 0.5f))
            .Group(Tween.Alpha(_coffeeImage, 0f, 0.5f));
    }

    public async Task ShowEmote(bool isHappy)
    {
        _emoteImage.sprite = isHappy ? _emotes.GetHappyEmote() : _emotes.GetSadEmote();

        await Sequence.Create().Group(Tween.Alpha(_emoteImage, 1f, 0.5f));

        await Tween.Delay(1.5f);

        await Sequence.Create().Group(Tween.Alpha(_emoteImage, 0f, 0.5f));
    }
}
