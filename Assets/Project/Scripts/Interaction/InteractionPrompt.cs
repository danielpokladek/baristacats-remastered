using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    private static float PROMPT_FADE_DURATION = 0.2f;

    [SerializeField]
    private Image _promptIcon;

    [SerializeField]
    private Image _interactionTimerIcon;

    private void Start()
    {
        _promptIcon.CrossFadeAlpha(0, 0, true);

        _interactionTimerIcon.CrossFadeAlpha(0, 0, true);
        _interactionTimerIcon.fillAmount = 0;
    }

    public void ShowPrompt()
    {
        _promptIcon.CrossFadeAlpha(1f, InteractionPrompt.PROMPT_FADE_DURATION, false);
    }

    public void ShowCircleFill()
    {
        _interactionTimerIcon.fillAmount = 0;
        _interactionTimerIcon.CrossFadeAlpha(1f, InteractionPrompt.PROMPT_FADE_DURATION, false);
    }

    public void ShowBoth()
    {
        ShowPrompt();
        ShowCircleFill();
    }

    public void HidePrompt()
    {
        _promptIcon.CrossFadeAlpha(0f, InteractionPrompt.PROMPT_FADE_DURATION, false);
    }

    public void HideCircleFill()
    {
        _interactionTimerIcon.CrossFadeAlpha(0f, InteractionPrompt.PROMPT_FADE_DURATION, false);
    }

    public void HideBoth()
    {
        HidePrompt();
        HideCircleFill();
    }

    public void UpdateTimerFill(float fillValue) => _interactionTimerIcon.fillAmount = fillValue;
}
