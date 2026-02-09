using PrimeTween;
using UnityEngine;

public enum MiniGameEnum
{
    FROTHING,
}

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    private GameUI _gameUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of `MiniGameManager` found in scene!");
            return;
        }

        Instance = this;

        _gameUI = GameUI.Instance;
    }

    public void StartMiniGame(MiniGameEnum miniGame)
    {
        switch (miniGame)
        {
            case MiniGameEnum.FROTHING:
                StartFrothingMiniGame();
                break;
        }
    }

    public async void StartFrothingMiniGame()
    {
        await Sequence.Create().Group(_gameUI.FadeInBlur()).Group(_gameUI.TransitionFrothingIn());
    }
}
