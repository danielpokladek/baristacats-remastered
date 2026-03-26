using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SanityUI _sanityUI;

    [SerializeField]
    private GameUI _gameUI;

    private int _failedCoffee = 0;
    private int _totalCoffee = 0;
    private int _customersQuit = 0;
    private int _successfulCoffee = 0;
    private int _rushHourCompleted = 0;

    private void Start()
    {
        var appManager = ApplicationManager.Instance;

        if (!appManager)
        {
            Debug.LogError("Unable to initialize GameManager, no ApplicationManager found!");
            return;
        }

        SanityController = new(appManager.CurrentDifficulty);
        RushController = new(appManager.CurrentDifficulty, this);

        Events.OnGameStart.AddListener(() =>
        {
            _failedCoffee = 0;
            _customersQuit = 0;
            _successfulCoffee = 0;
            _totalCoffee = 0;
            _rushHourCompleted = 0;

            ControlsManager.EnablePlayerControls();
            enabled = true;

            RushController.StartTimer();
        });
        Events.OnGameOver.AddListener(() =>
        {
            RushController.StopTimer();

            ControlsManager.DisablePlayerControls();
            ControlsManager.DisableFrothingControls();
            enabled = false;
        });

        Events.CustomerEvents.OnOutOfTime.AddListener((_) => _customersQuit++);
        Events.CustomerEvents.OnOrderFailed.AddListener(
            (_) =>
            {
                _failedCoffee++;
                _totalCoffee++;
            }
        );
        Events.CustomerEvents.OnOrderSuccessful.AddListener(
            (_) =>
            {
                _successfulCoffee++;
                _totalCoffee++;
            }
        );

        Events.RushEnd.AddListener(() => _rushHourCompleted++);

        enabled = false;
    }

    // private void Update()
    // {
    //     RushController.Update();
    // }

    public int CoffeeCompleted { get; private set; }

    public SanityController SanityController { get; private set; }
    public RushController RushController { get; private set; }

    public int CoffeeFailed => _failedCoffee;
    public int CoffeeSuccessful => _successfulCoffee;
    public int CoffeeTotal => _totalCoffee;
    public int CustomerQuit => _customersQuit;

    public int RushHourComplete => _rushHourCompleted;
    public int DrinksCompleted => _totalCoffee;
}
