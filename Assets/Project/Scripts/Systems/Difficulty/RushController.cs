#nullable enable
using UnityEngine;

public class RushController
{
    private float _timeToStart;
    private float _duration;

    private float _timer;

    private bool _rushActive;

    private DifficultySettingsSO _difficultySettings;

    public RushController(DifficultySettingsSO difficultySettings)
    {
        _difficultySettings = difficultySettings;

        _timeToStart = _difficultySettings.RushModeWaitTime.Max;
        _duration = _difficultySettings.RushModeDuration.Min;

        _timer = 0f;
        _rushActive = false;
    }

    public bool IsRushActive => _rushActive;
    public float ProgressToRush => Mathf.Clamp01(_timer / _timeToStart);

    public void Update()
    {
        _timer += Time.deltaTime;

        if (_rushActive)
        {
            if (_timer > _duration)
            {
                EndRush();
                return;
            }
        }
        else
        {
            if (_timer > _timeToStart)
            {
                StartRush();
                return;
            }
        }
    }

    private void StartRush()
    {
        _timer = 0;
        _duration = _difficultySettings.RushModeDuration.GetRandomValue();

        _rushActive = true;

        Events.RushStart.Invoke();
    }

    private void EndRush()
    {
        _timer = 0;
        _timeToStart = _difficultySettings.RushModeWaitTime.GetRandomValue();

        _rushActive = false;

        Events.RushEnd.Invoke();
    }
}
