using PrimeTween;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [SerializeField]
    private TMP_Text _completedCoffeeText;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of `GameUIManager` found in scene!");
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void UpdateCompletedCoffeeText(int newCoffeeCount)
    {
        _completedCoffeeText.SetText(newCoffeeCount.ToString());
    }
}
