using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CustomerMovement))]
public class CustomerController : MonoBehaviour
{
    [SerializeField]
    private Image _emoteImage;

    [SerializeField]
    private Image _coffeeImage;

    public CoffeeData DesiredCoffee { get; private set; }

    public CustomerMovement Movement { get; private set; }

    private CharacterEmotes _emotes;

    private ApplicationManager _appManager;
    private DifficultyController _difficultyController;

    private OrderTicketUI _ticket;

    private float _patience = 0f;
    private float _patienceTimer = 0f;

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        _patienceTimer -= Time.deltaTime;
        _ticket.SetPatienceBarFill(_patienceTimer / _patience);

        if (_patienceTimer <= 0)
        {
            enabled = false;

            Events.CustomerEvents.RanOutOfPatience.Invoke(this);
        }
    }

    public void Setup(ApplicationManager appManager, DifficultyController difficultyController)
    {
        _appManager = appManager;
        _difficultyController = difficultyController;

        _emotes = CharacterEmotes.Instance;

        Movement = GetComponent<CustomerMovement>();

        _emoteImage.color = new(1, 1, 1, 0);
        _coffeeImage.color = new(1, 1, 1, 0);

        DesiredCoffee = new();
    }

    public void Initialize(OrderTicketUI ticket)
    {
        _patience = CalculatePatience();
        _patienceTimer = _patience;

        _ticket = ticket;
    }

    public void StartTimer()
    {
        enabled = true;
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

    private float CalculatePatience()
    {
        var difficultySettings = _appManager.CurrentDifficulty;
        var currentDifficulty = _difficultyController.CurrentDifficulty;

        return difficultySettings.CustomerPatience.GetRandomValue()
            / (1f + currentDifficulty * difficultySettings.CustomerPatienceScaling);
    }
}
