using System.Threading.Tasks;
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
        SanityController = new(appManager.CurrentDifficulty, DifficultyController, _sanityUI);
        RushController = new();
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
