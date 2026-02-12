using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameUI _gameUI;

    private void Start()
    {
        var appManager = ApplicationManager.Instance;

        if (!appManager)
        {
            Debug.LogError("Unable to initialize GameManager, no ApplicationManager found!");
            return;
        }

        DifficultyController = new DifficultyController(appManager);
        _gameUI = GameUI.Instance;
    }

    private void Update()
    {
        DifficultyController.UpdateDifficultyStep();
    }

    public int CoffeeCompleted { get; private set; }
    public DifficultyController DifficultyController { get; private set; }

    public async Task ProcessCustomerServed(CustomerController customer)
    {
        var desiredCoffee = customer.DesiredCoffee;
        var servedCoffee = customer.ServedCoffee;

        var isHappy = true;

        if (desiredCoffee.Milk != servedCoffee.Milk)
        {
            isHappy = false;
        }

        if (desiredCoffee.Quality > servedCoffee.Quality)
        {
            isHappy = false;
        }

        CoffeeCompleted++;

        _gameUI.UpdateCompletedCoffeeText(CoffeeCompleted);

        await customer.ShowEmote(isHappy);
    }
}
