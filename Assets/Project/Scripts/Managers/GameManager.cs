using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SanityUI _sanityUI;

    [SerializeField]
    private GameUI _gameUI;

    private void Start()
    {
        var appManager = ApplicationManager.Instance;

        if (!appManager)
        {
            Debug.LogError("Unable to initialize GameManager, no ApplicationManager found!");
            return;
        }

        DifficultyController = new(appManager);
        SanityController = new(appManager.CurrentDifficulty);
        RushController = new(appManager.CurrentDifficulty);

        Events.OnGameStart.AddListener(() =>
        {
            ControlsManager.EnablePlayerControls();
            enabled = true;
        });
        Events.OnGameOver.AddListener(() =>
        {
            ControlsManager.DisablePlayerControls();
            ControlsManager.DisableFrothingControls();
            enabled = false;
        });

        enabled = false;
    }

    private void Update()
    {
        RushController.Update();
        DifficultyController.UpdateDifficultyStep();
    }

    public int CoffeeCompleted { get; private set; }

    public DifficultyController DifficultyController { get; private set; }
    public SanityController SanityController { get; private set; }
    public RushController RushController { get; private set; }
}
