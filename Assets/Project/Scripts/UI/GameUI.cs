using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("References")]
    [SerializeField]
    private TMP_Text _completedCoffeeText;

    [SerializeField]
    private Volume _globalVolume;

    [Header("Animations")]
    [SerializeField]
    private float _miniGameTransition = 0.25f;

    [Header("Mini Game")]
    [SerializeField]
    private RectTransform _frothingMiniGameTransform;

    private DepthOfField _dof;

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

        _globalVolume.profile.TryGet(out DepthOfField dof);

        if (!dof)
            Debug.LogError("No DepthOfField found on global volume!");

        _dof = dof;

        TransitionFrothingOut(true);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            TransitionFrothingIn();
            FadeInBlur();
            return;
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            TransitionFrothingOut();
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
            Screen.height,
            isInstant ? 0 : _miniGameTransition
        );
    }
}
