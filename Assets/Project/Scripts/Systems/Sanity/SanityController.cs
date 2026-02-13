#nullable enable

using UnityEngine;

public class SanityController
{
    private float _maxSanity;
    private float _currentSanity;

    private SanityUI _sanityUI;
    private DifficultySettingsSO _difficulty;
    private DifficultyController _difficultyController;

    public SanityController(
        DifficultySettingsSO difficulty,
        DifficultyController difficultyController,
        SanityUI sanityUI
    )
    {
        _difficultyController = difficultyController;
        _difficulty = difficulty;

        _maxSanity = difficulty.StartingSanity;
        _currentSanity = _maxSanity;
        _sanityUI = sanityUI;

        Events.CustomerEvents.OrderFailed.AddListener(HandleOrderFailed);
        Events.CustomerEvents.OrderSuccessful.AddListener(HandleOrderSuccessful);
    }

    private void HandleOrderFailed(CustomerController _)
    {
        _currentSanity -= GetSanityPenalty();
        _sanityUI.UpdateSanityUI(1f - (_currentSanity / _maxSanity));

        Debug.Log($"Sanity dropped! Current sanity: {_currentSanity}");

        if (_currentSanity <= 0)
        {
            Debug.Log("All sanity lost, poor Milky..");
        }
    }

    public void HandleOrderSuccessful(CustomerController _)
    {
        _currentSanity += _difficulty.SanityGainPerCustomer;

        if (_currentSanity >= _maxSanity)
        {
            _currentSanity = _maxSanity;
        }

        _sanityUI.UpdateSanityUI(1f - (_currentSanity / _maxSanity));

        Debug.Log($"Sanity gained! Current sanity: {_currentSanity}");
    }

    private float GetSanityPenalty()
    {
        return _difficulty.SanityLossPerCustomer
            * (1f + _difficultyController.CurrentDifficulty * _difficulty.SanityScaling);
    }
}
