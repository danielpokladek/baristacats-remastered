class ApplicationSettings
{
    public DifficultySettingsSO CurrentDifficulty { get; private set; }

    public void SetDifficulty(DifficultySettingsSO difficulty)
    {
        CurrentDifficulty = difficulty;
    }
}
