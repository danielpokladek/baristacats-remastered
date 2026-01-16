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

    public void Show()
    {
        _interactionTimerIcon.fillAmount = 0;

        _promptIcon.CrossFadeAlpha(1f, InteractionPrompt.PROMPT_FADE_DURATION, false);
        _interactionTimerIcon.CrossFadeAlpha(1f, InteractionPrompt.PROMPT_FADE_DURATION, false);
    }

    public void Hide()
    {
        _promptIcon.CrossFadeAlpha(0f, InteractionPrompt.PROMPT_FADE_DURATION, false);
        _interactionTimerIcon.CrossFadeAlpha(0f, InteractionPrompt.PROMPT_FADE_DURATION, false);
    }

    public void UpdateTimerFill(float fillValue) => _interactionTimerIcon.fillAmount = fillValue;
}
