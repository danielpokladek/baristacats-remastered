#nullable enable
using PrimeTween;
using UnityEngine;

public class RushController
{
    private readonly DifficultySettingsSO _difficulty;
    private readonly GameManager _gameManager;

    private float _timeToStart;
    private float _duration;

    private bool _rushActive;

    private Tween? _currentTimer;

    public RushController(DifficultySettingsSO difficultySettings, GameManager gameManager)
    {
        _difficulty = difficultySettings;
        _gameManager = gameManager;

        _timeToStart = _difficulty.RushModeWaitTime.Max;
        _duration = _difficulty.RushModeDuration.Min;

        _rushActive = false;
    }

    public bool IsRushActive => _rushActive;
    public float ProgressToRush => _currentTimer?.progressTotal ?? 0;

    public void StartTimer()
    {
        var duration = _rushActive ? _duration : _timeToStart;

        _currentTimer = Tween
            .Custom(0, duration, duration, (_) => { })
            .OnComplete(() =>
            {
                if (_rushActive)
                    EndRush();
                else
                    StartRush();
            });
    }

    public void StopTimer()
    {
        _currentTimer?.Stop();
    }

    private void StartRush()
    {
        var baseDuration = _difficulty.RushModeDuration.GetRandomValue();
        var modifier = _gameManager.RushHourComplete / _difficulty.RushModeScaling;
        modifier = Mathf.Max(1, modifier);
        modifier = _difficulty.RushModeModifier * modifier;

        _duration = baseDuration * modifier;

        _rushActive = true;

        Events.RushStart.Invoke();
        StartTimer();
    }

    private void EndRush()
    {
        var baseDuration = _difficulty.RushModeWaitTime.GetRandomValue();
        var modifier = _gameManager.RushHourComplete / _difficulty.RushModeScaling;
        modifier = Mathf.Max(1, modifier);
        modifier = _difficulty.RushModeModifier * modifier;

        _timeToStart = Mathf.Max(baseDuration / modifier, 10);

        _rushActive = false;

        Events.RushEnd.Invoke();
        StartTimer();
    }
}
