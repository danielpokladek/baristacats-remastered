#nullable enable
using PrimeTween;

public class RushController
{
    private float _timeToStart;
    private float _duration;

    private bool _rushActive;

    private DifficultySettingsSO _difficultySettings;

    private Tween? _currentTimer;

    public RushController(DifficultySettingsSO difficultySettings)
    {
        _difficultySettings = difficultySettings;

        _timeToStart = _difficultySettings.RushModeWaitTime.Max;
        _duration = _difficultySettings.RushModeDuration.Min;

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
        _duration = _difficultySettings.RushModeDuration.GetRandomValue();

        _rushActive = true;

        Events.RushStart.Invoke();
        StartTimer();
    }

    private void EndRush()
    {
        _timeToStart = _difficultySettings.RushModeWaitTime.GetRandomValue();

        _rushActive = false;

        Events.RushEnd.Invoke();
        StartTimer();
    }
}
