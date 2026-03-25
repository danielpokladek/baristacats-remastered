#nullable enable
using UnityEngine;

public class SanityController
{
    private float _maxSanity;
    private float _currentSanity;

    private readonly DifficultySettingsSO _difficulty;

    public SanityController(DifficultySettingsSO difficulty)
    {
        _difficulty = difficulty;

        Events.OnGameStart.AddListener(HandleGameStart);

        Events.CustomerEvents.OnOrderFailed.AddListener(HandleOrderFailed);
        Events.CustomerEvents.OnOutOfTime.AddListener(HandleOrderFailed);
        Events.CustomerEvents.OnOrderSuccessful.AddListener(HandleOrderSuccessful);
    }

    private void HandleGameStart()
    {
        _maxSanity = _difficulty.StartingSanity;
        _currentSanity = _maxSanity;

        HandleSanityChange();
    }

    private void HandleOrderFailed(CustomerController _)
    {
        if (_currentSanity == 1)
            _currentSanity -= _difficulty.SanityLossPerCustomer;
        else
            _currentSanity -= GetSanityPenalty();

        HandleSanityChange();

        if (_currentSanity <= 0)
            Events.OnGameOver.Invoke();
    }

    public void HandleOrderSuccessful(CustomerController _)
    {
        _currentSanity += _difficulty.SanityGainPerCustomer;

        if (_currentSanity >= _maxSanity)
        {
            _currentSanity = _maxSanity;
        }

        HandleSanityChange();
    }

    private void HandleSanityChange()
    {
        var sanityProgress = 1f - (_currentSanity / _maxSanity);
        Events.OnSanityChange.Invoke(sanityProgress);
    }

    private float GetSanityPenalty()
    {
        var sanityLossProgress = 1 - (_currentSanity / _difficulty.StartingSanity);

        return _difficulty.SanityLossPerCustomer
            * _difficulty.SanityLossCurve.Evaluate(sanityLossProgress);
    }
}
