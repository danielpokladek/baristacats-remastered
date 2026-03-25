#nullable enable

using UnityEngine;

public class DifficultyController
{
    private ApplicationManager _appManager;

    private float _gameTime;

    // private float _difficultyRampDuration;
    private int _currentDifficulty;

    private bool _rushActive;

    public int CurrentDifficulty => _currentDifficulty;

    public DifficultyController(ApplicationManager appManager)
    {
        _appManager = appManager;

        _gameTime = 0;

        // _difficultyRampDuration = _appManager.CurrentDifficulty.DifficultyRampDuration;
        _currentDifficulty = 1;

        _rushActive = false;

        Events.RushStart.AddListener(() => _rushActive = true);
        Events.RushEnd.AddListener(() => _rushActive = false);
    }

    public void UpdateDifficultyStep()
    {
        if (_rushActive)
            return;

        _gameTime += Time.deltaTime;

        // var newDifficulty = Mathf.CeilToInt(_gameTime / _difficultyRampDuration);

        // if (newDifficulty == _currentDifficulty)
        //     return;

        // _currentDifficulty = newDifficulty;
        // Events.OnDifficultyLevelChange.Invoke(newDifficulty);
    }
}
