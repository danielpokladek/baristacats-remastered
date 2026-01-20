using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameUIManager _gameUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of `CharacterEmotes` found in scene!");
            return;
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _gameUI = GameUIManager.Instance;
    }

    public int CoffeeCompleted { get; private set; }

    public async Task ProcessCustomerServed(CustomerController customer)
    {
        CoffeeCompleted++;

        _gameUI.UpdateCompletedCoffeeText(CoffeeCompleted);

        await customer.ShowEmote(true);
    }
}
