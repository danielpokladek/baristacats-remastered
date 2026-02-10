using PrimeTween;
using UnityEngine;

public enum MiniGameEnum
{
    FROTHING,
    MILK_PICKING,
}

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [SerializeField]
    private GameUI _gameUI;

    [SerializeField]
    private PlayerController _player;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of `MiniGameManager` found in scene!");
            return;
        }

        Instance = this;
    }

    public void StartMiniGame(MiniGameEnum miniGame)
    {
        switch (miniGame)
        {
            case MiniGameEnum.FROTHING:
                StartFrothing();
                break;

            case MiniGameEnum.MILK_PICKING:
                StartMilkPicking();
                break;
        }
    }

    public async void StartFrothing()
    {
        ControlsManager.PlayerActions.Disable();

        await Sequence.Create().Group(_gameUI.FadeInBlur()).Group(_gameUI.TransitionFrothingIn());

        ControlsManager.FrothingActions.Enable();
    }

    public async void StartMilkPicking()
    {
        if (_player.Inventory.IsHoldingItem)
            return;

        ControlsManager.PlayerActions.Disable();

        await Sequence.Create().Group(_gameUI.FadeInBlur()).Group(_gameUI.TransitionPickingIn());

        ControlsManager.MilkPickingActions.Enable();
    }

    public async void HandleMilkPicked(MilkType milkType)
    {
        ControlsManager.MilkPickingActions.Disable();

        await Sequence.Create().Group(_gameUI.FadeOutBlur()).Group(_gameUI.TransitionPickingOut());

        _player.Inventory.MilkInHand = milkType;

        ControlsManager.PlayerActions.Enable();
    }

    public async void HandleMilkFrothed(int qualityDeduction)
    {
        ControlsManager.FrothingActions.Disable();

        await Sequence.Create().Group(_gameUI.FadeOutBlur()).Group(_gameUI.TransitionFrothingOut());

        var coffeeInHand = _player.Inventory.CoffeeInHand;

        coffeeInHand.Quality -= qualityDeduction;
        coffeeInHand.Milk = _player.Inventory.MilkInHand;
        _player.Inventory.MilkInHand = MilkType.NONE;

        ControlsManager.PlayerActions.Enable();
    }
}
