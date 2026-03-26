using PrimeTween;
using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    [SerializeField]
    GameManager _gameManager;

    [SerializeField]
    CanvasGroup _canvasGroup;

    [SerializeField]
    TMPro.TMP_Text _statsText;

    private void Awake()
    {
        Events.OnShowCredits.AddListener(HandleShowCredits);
    }

    private void HandleShowCredits()
    {
        string text = "";
        text += $"You have brewed {_gameManager.CoffeeTotal} coffees. ";
        text +=
            $"{_gameManager.CoffeeSuccessful} coffees have been successful, and {_gameManager.CoffeeFailed} no so much.";
        text += "\n\n";
        text +=
            $"After walking out, {_gameManager.CustomerQuit} will definitely not be coming back.";
        text += "\n\n";
        text += "Thanks for playing!";

        _statsText.text = text;

        Tween.Alpha(_canvasGroup, 1f, 0.2f);
    }
}
