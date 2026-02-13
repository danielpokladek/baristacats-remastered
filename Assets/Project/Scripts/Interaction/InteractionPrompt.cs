using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    private static float PROMPT_FADE_DURATION = 0.2f;

    [SerializeField]
    private Image _promptIcon;

    private void Start()
    {
        _promptIcon.CrossFadeAlpha(0, 0, true);
    }

    public void ShowPrompt()
    {
        _promptIcon.CrossFadeAlpha(1f, PROMPT_FADE_DURATION, false);
    }

    public void HidePrompt()
    {
        _promptIcon.CrossFadeAlpha(0f, PROMPT_FADE_DURATION, false);
    }
}
