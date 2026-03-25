using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SanityUI _sanityUI;

    [SerializeField]
    private GameUI _gameUI;

    private int _failedBrews = 0;
    private int _drinksCompleted = 0;
    private int _customersQuit = 0;
    private int _successfulBrews = 0;
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
        RushController = new(appManager.CurrentDifficulty);

        Events.OnGameStart.AddListener(() =>
        {
            _failedBrews = 0;
            _customersQuit = 0;
            _successfulBrews = 0;
            _drinksCompleted = 0;
            _rushHourCompleted = 0;

            ControlsManager.EnablePlayerControls();
            enabled = true;
        });
        Events.OnGameOver.AddListener(() =>
        {
            ControlsManager.DisablePlayerControls();
            ControlsManager.DisableFrothingControls();
            enabled = false;

            Debug.Log("Milky has served:");
            Debug.Log($"{_successfulBrews} happy customers!");
            Debug.Log($"{_failedBrews} not so happy customers.");
            Debug.Log($"{_customersQuit} customers probably won't come back.");
        });

        Events.CustomerEvents.OnOutOfTime.AddListener((_) => _customersQuit++);
        Events.CustomerEvents.OnOrderFailed.AddListener(
            (_) =>
            {
                _failedBrews++;
                _drinksCompleted++;
            }
        );
        Events.CustomerEvents.OnOrderSuccessful.AddListener(
            (_) =>
            {
                _successfulBrews++;
                _drinksCompleted++;
            }
        );

        Events.RushEnd.AddListener(() => _rushHourCompleted++);

        enabled = false;
    }

    private void Update()
    {
        RushController.Update();
    }

    public int CoffeeCompleted { get; private set; }

    public SanityController SanityController { get; private set; }
    public RushController RushController { get; private set; }

    public int RushHourComplete => _rushHourCompleted;
    public int DrinksCompleted => _drinksCompleted;
}
