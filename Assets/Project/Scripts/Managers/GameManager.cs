using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameUI _gameUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of `GameManager` found in scene!");
            return;
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _gameUI = GameUI.Instance;
    }

    public int CoffeeCompleted { get; private set; }

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
