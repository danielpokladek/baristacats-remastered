#nullable enable
using UnityEngine;

public class RushController
{
    private float _timeToRush;
    private float _rushDuration;

    private float _rushTimer;

    private bool _rushActive;

    private DifficultySettingsSO _difficultySettings;

    public RushController(DifficultySettingsSO difficultySettings)
    {
        _difficultySettings = difficultySettings;

        _timeToRush = _difficultySettings.RushModeWaitTime.Max;
        _rushDuration = _difficultySettings.RushModeDuration.Min;

        _rushTimer = 0f;
        _rushActive = false;
    }

    public bool IsRushActive => _rushActive;
    public float ProgressToRush => Mathf.Clamp01(_rushTimer / _timeToRush);

    public void Update()
    {
        _rushTimer += Time.deltaTime;

        if (_rushActive)
        {
            if (_rushTimer > _rushDuration)
            {
                EndRush();
                return;
            }
        }
        else
        {
            if (_rushTimer > _timeToRush)
            {
                StartRush();
                return;
            }
        }
    }

    private void StartRush()
    {
        _rushActive = true;

        Events.RushStart.Invoke();
    }

    private void EndRush()
    {
        _rushTimer = 0;
        _rushActive = false;

        _timeToRush = _difficultySettings.RushModeWaitTime.GetRandomValue();
        _rushDuration = _difficultySettings.RushModeDuration.GetRandomValue();

        Events.RushEnd.Invoke();
    }
}
