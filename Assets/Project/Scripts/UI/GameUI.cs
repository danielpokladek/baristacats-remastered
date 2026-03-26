using System.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    TMP_Text _completedCoffeeText;

    [SerializeField]
    Volume _globalVolume;

    [SerializeField]
    CanvasGroup _rushHour;

    [SerializeField]
    CanvasGroup _rushOver;

    [SerializeField]
    CanvasGroup _failVideoCanvasGroup;

    [SerializeField]
    VideoPlayer _failVideoPlayer;

    [Header("Animations")]
    [SerializeField]
    float _miniGameTransition = 0.25f;

    [Header("Mini Game")]
    [SerializeField]
    RectTransform _frothingMiniGameTransform;

    [SerializeField]
    RectTransform _milkPickingMiniGame;

    private DepthOfField _dof;

    private void Awake()
    {
        _globalVolume.profile.TryGet(out DepthOfField dof);

        if (!dof)
            Debug.LogError("No DepthOfField found on global volume!");

        _dof = dof;

        TransitionFrothingOut(true);
        TransitionPickingOut(true);

        _failVideoPlayer.loopPointReached += HandleFailVideoFinished;

        Events.RushStart.AddListener(ShowRush);
        Events.RushEnd.AddListener(ShowRushOver);

        Events.OnGameStart.AddListener(HandleGameStart);
        Events.OnGameOver.AddListener(HandleGameOver);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            TransitionPickingIn();
            FadeInBlur();
            return;
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            TransitionPickingOut();
            FadeOutBlur();
            return;
        }
    }
#endif

    public void UpdateCompletedCoffeeText(int newCoffeeCount)
    {
        _completedCoffeeText.SetText(newCoffeeCount.ToString());
    }

    public Tween FadeInBlur()
    {
        return Tween.Custom(
            3f,
            1.5f,
            duration: _miniGameTransition,
            onValueChange: val => _dof.focusDistance.value = val
        );
    }

    public Tween FadeOutBlur(bool isInstant = false)
    {
        return Tween.Custom(
            1.5f,
            3f,
            duration: isInstant ? 0 : _miniGameTransition,
            onValueChange: val => _dof.focusDistance.value = val
        );
    }

    public Tween TransitionFrothingIn()
    {
        return Tween.LocalPositionY(_frothingMiniGameTransform.transform, 0, _miniGameTransition);
    }

    public Tween TransitionFrothingOut(bool isInstant = false)
    {
        return Tween.LocalPositionY(
            _frothingMiniGameTransform,
            -Screen.height * 2,
            isInstant ? 0 : _miniGameTransition
        );
    }

    public Tween TransitionPickingIn()
    {
        return Tween.LocalPositionY(_milkPickingMiniGame.transform, 0, _miniGameTransition);
    }

    public Tween TransitionPickingOut(bool isInstant = false)
    {
        return Tween.LocalPositionY(
            _milkPickingMiniGame,
            -Screen.height * 2,
            isInstant ? 0 : _miniGameTransition
        );
    }

    private async void ShowRush()
    {
        await Tween.Alpha(_rushHour, 1f, 0.15f);

        await Tween.Delay(2f);

        await Tween.Alpha(_rushHour, 0f, 0.15f);
    }

    private async void ShowRushOver()
    {
        await Tween.Alpha(_rushOver, 1f, 0.15f);

        await Tween.Delay(2f);

        await Tween.Alpha(_rushOver, 0f, 0.15f);
    }

    private void HandleGameStart()
    {
        _failVideoCanvasGroup.alpha = 0;
    }

    private async void HandleGameOver()
    {
        if (_failVideoPlayer.isPlaying)
            return;

        await Tween.Alpha(_failVideoCanvasGroup, 1f, 0.2f);

        _failVideoPlayer.Stop();
        _failVideoPlayer.Play();
    }

    private void HandleFailVideoFinished(VideoPlayer _)
    {
        Events.OnShowCredits.Invoke();
    }
}
