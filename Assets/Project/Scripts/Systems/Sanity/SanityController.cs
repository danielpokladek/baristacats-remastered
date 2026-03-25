#nullable enable

using UnityEngine;

public class SanityController
{
    private float _maxSanity;
    private float _currentSanity;

    private DifficultySettingsSO _difficulty;

    public SanityController(DifficultySettingsSO difficulty)
    {
        _difficulty = difficulty;

        _maxSanity = difficulty.StartingSanity;
        _currentSanity = _maxSanity;

        Events.CustomerEvents.OnOrderFailed.AddListener(HandleOrderFailed);
        Events.CustomerEvents.OnOutOfTime.AddListener(HandleOrderFailed);
        Events.CustomerEvents.OnOrderSuccessful.AddListener(HandleOrderSuccessful);
    }

    private void HandleOrderFailed(CustomerController _)
    {
        if (_currentSanity == 1)
            _currentSanity -= _difficulty.SanityLossPerCustomer;
        else
            _currentSanity -= GetSanityPenalty();

        var sanityProgress = 1f - (_currentSanity / _maxSanity);
        Events.OnSanityChange.Invoke(sanityProgress);

        Debug.Log($"Sanity lost, current sanity: {_currentSanity}");

        if (_currentSanity <= 0)
        {
            Debug.Log("All sanity lost, poor Milky..");
            Events.OnGameOver.Invoke();
        }
    }

    public void HandleOrderSuccessful(CustomerController _)
    {
        _currentSanity += _difficulty.SanityGainPerCustomer;

        if (_currentSanity >= _maxSanity)
        {
            _currentSanity = _maxSanity;
        }

        var sanityProgress = 1f - (_currentSanity / _maxSanity);
        Events.OnSanityChange.Invoke(sanityProgress);

        Debug.Log($"Sanity gained, current sanity: {_currentSanity}");
    }

    private float GetSanityPenalty()
    {
        var sanityLossProgress = 1 - (_currentSanity / _difficulty.StartingSanity);

        return _difficulty.SanityLossPerCustomer
            * _difficulty.SanityLossCurve.Evaluate(sanityLossProgress);
    }
}
