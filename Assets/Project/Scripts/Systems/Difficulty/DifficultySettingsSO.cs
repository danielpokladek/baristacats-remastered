using UnityEngine;

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
    public float BaseCustomerSpawnInterval;
    public float CustomerSpawnScaling;

    public float BaseCustomerPatience;
    public float CustomerPatienceScaling;

    [Header("Drinks Difficulty Settings")]
    public int BaseDrinkComplexity;
    public int DrinkComplexityScaling;

    [Header("Sanity Difficulty Settings")]
    public float SanityLossPerCustomer;
    public float SanityGainPerCustomer;
    public float SanityScaling;
}
