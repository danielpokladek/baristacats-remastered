using System;
using UnityEngine;

[Serializable]
public class MinMax
{
    [SerializeField]
    private float _minimum;

    [SerializeField]
    private float _maximum;

    [SerializeField]
    private AnimationCurve _curve;

    public float Max => _maximum;
    public float Min => _minimum;

    /// <summary>
    /// Gets value based on progress.
    /// </summary>
    /// <param name="progress">Progress used to get value.</param>
    /// <returns>A value between min and max by evaluating it against animation curve.</returns>
    public float GetValue(float progress)
    {
        float curveValue = _curve.Evaluate(Mathf.Clamp01(progress));
        return Mathf.Lerp(_minimum, _maximum, curveValue);
    }

    /// <summary>
    /// Gets a random value.
    /// </summary>
    /// <returns>A random value between the min and max.</returns>
    public float GetRandomValue()
    {
        return UnityEngine.Random.Range(_minimum, _maximum);
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
    public MinMax RushModeWaitTime;
    public MinMax RushModeDuration;
    public float RushModeScaling;
    public float RushModeModifier;

    [Header("Customer Difficulty Settings")]
    public MinMax CustomerSpawnInterval;
    public MinMax CustomerPatience;

    [Header("Queue Settings")]
    public int MaxOrdersRegular;
    public int MaxOrdersRush;

    [Header("Drinks Difficulty Settings")]
    public int BaseDrinkComplexity;
    public float DrinkComplexityScaling;

    [Header("Sanity Difficulty Settings")]
    public float StartingSanity;
    public float SanityLossPerCustomer;
    public float SanityGainPerCustomer;
    public AnimationCurve SanityLossCurve;
}
