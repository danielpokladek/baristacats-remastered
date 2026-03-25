using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;

[Serializable]
public class DifficultyOption
{
    public string Name;
    public DifficultySettingsSO Settings;
}

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance;

    [SerializeField]
    private List<DifficultyOption> _difficultyOptions = new();

    private ApplicationSettings _appSettings;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of ApplicationManager detected!");
            return;
        }

        PrimeTweenConfig.warnZeroDuration = false;

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _appSettings = new();

        // TODO: Remove the hardcode here, and save/load from file.
        _appSettings.SetDifficulty(_difficultyOptions.First().Settings);
    }

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        Events.OnGameStart.Invoke();
    }

    public List<DifficultyOption> DifficultyOptions => _difficultyOptions;
    public DifficultySettingsSO CurrentDifficulty => _appSettings.CurrentDifficulty;
}
