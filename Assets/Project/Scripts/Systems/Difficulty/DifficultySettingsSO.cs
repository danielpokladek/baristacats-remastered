using System;
using UnityEngine;

[Serializable]
public class MinMax
{
    public float MinimumValue;
    public float MaximumValue;

    public float GetRandomValue()
    {
        return UnityEngine.Random.Range(MinimumValue, MaximumValue);
    }
}

[CreateAssetMenu(
    fileName = "NewGameDifficulty",
    menuName = "Difficulty/Difficulty Settings",
    order = 0
)]
public class DifficultySettingsSO : ScriptableObject
{
    [Header("Difficulty Increase Settings")]
    public float DifficultyRampDuration;

    [Header("Customer Difficulty Settings")]
    public MinMax CustomerSpawnInterval;
    public float CustomerSpawnScaling;

    public MinMax CustomerPatience;
    public float CustomerPatienceScaling;

    [Header("Queue Settings")]
    public int MaxOrdersQueued;

    [Header("Drinks Difficulty Settings")]
    public int BaseDrinkComplexity;
    public float DrinkComplexityScaling;

    [Header("Sanity Difficulty Settings")]
    public float StartingSanity;
    public float SanityLossPerCustomer;
    public float SanityGainPerCustomer;
    public float SanityScaling;
}
