using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField]
    private float _transitionDuration = 0.25f;

    [SerializeField]
    private Camera _frothingCamera;

    [SerializeField]
    private Vector3 _frothingCameraHiddenPosition;

    [SerializeField]
    private Vector3 _frothingCameraShownPosition;

    [SerializeField]
    private Volume _globalVolume;

    private DepthOfField _dof;

    private void Awake()
    {
        _globalVolume.profile.TryGet(out DepthOfField dof);

        if (!dof)
            Debug.LogError("No DepthOfField found on global volume!");

        _dof = dof;

        _frothingCamera.transform.position = _frothingCameraHiddenPosition;

        Events.MiniGameEvents.OnFrothingTransitionIn.AddListener(() => TransitionFrothingIn());
        Events.MiniGameEvents.OnFrothingTransitionOut.AddListener(() => TransitionFrothingOut());
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            TransitionFrothingIn();
            return;
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            TransitionFrothingOut();
            return;
        }
    }
#endif

    private async void TransitionFrothingIn()
    {
        ControlsManager.DisablePlayerControls();

        await Sequence
            .Create()
            .Group(
                Tween.Position(
                    _frothingCamera.transform,
                    _frothingCameraShownPosition,
                    _transitionDuration
                )
            )
            .Group(
                Tween.Custom(
                    3f,
                    1.5f,
                    duration: _transitionDuration,
                    ease: Ease.InQuart,
                    onValueChange: val => _dof.focusDistance.value = val
                )
            );

        Events.MiniGameEvents.OnFrothingStart.Invoke();
    }

    private async void TransitionFrothingOut()
    {
        await Sequence
            .Create()
            .Group(
                Tween.Position(
                    _frothingCamera.transform,
                    _frothingCameraHiddenPosition,
                    _transitionDuration
                )
            )
            .Group(
                Tween.Custom(
                    1.5f,
                    3f,
                    duration: _transitionDuration,
                    onValueChange: val => _dof.focusDistance.value = val
                )
            );

        ControlsManager.EnablePlayerControls();
    }

#if UNITY_EDITOR
    [ContextMenu("Transition to Frothing Mini-Game")]
    public void TransitionToFrothingMiniGame()
    {
        Events.MiniGameEvents.OnFrothingTransitionIn.Invoke();
    }
#endif
}
