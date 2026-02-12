using System;
using System.Collections.Generic;
using System.Linq;
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

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _appSettings = new();
        _appSettings.SetDifficulty(_difficultyOptions.First().Settings);
    }

    public List<DifficultyOption> DifficultyOptions => _difficultyOptions;
    public DifficultySettingsSO CurrentDifficulty => _appSettings.CurrentDifficulty;
}
