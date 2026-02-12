#nullable enable

using UnityEngine;

public class DifficultyController
{
    private ApplicationManager _appManager;

    private float _gameTime = 0f;

    private float _difficultyRampDuration = 0f;
    private int _currentDifficulty = 0;

    public int CurrentDifficulty => _currentDifficulty;

    public DifficultyController(ApplicationManager appManager)
    {
        _appManager = appManager;
        _difficultyRampDuration = _appManager.CurrentDifficulty.DifficultyRampDuration;

        Events.OnDifficultyLevelChange.AddListener(
            (level) => Debug.Log($"Difficulty level changed to: {level}")
        );
    }

    public void UpdateDifficultyStep()
    {
        _gameTime += Time.deltaTime;

        var newDifficulty = Mathf.CeilToInt(_gameTime / _difficultyRampDuration);

        if (newDifficulty == _currentDifficulty)
            return;

        _currentDifficulty = newDifficulty;
        Events.OnDifficultyLevelChange.Invoke(newDifficulty);
    }
}
