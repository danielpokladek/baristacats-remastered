using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class MilkFrothingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FrotherNozzle _nozzle;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Foam Properties")]
    [SerializeField]
    private Image _foamProgressBar;

    [SerializeField]
    private float _foamGainSpeed = 0.35f;

    [SerializeField]
    private float _foamPerfectMinProgress = 0.66f;

    [SerializeField]
    private float _foamPerfectMaxProgress = 0.75f;

    [Header("Stir Properties")]
    [SerializeField]
    private Image _stirProgressBar;

    [SerializeField]
    private float _stirGainSpeed = 0.7f;

    private float _temperatureProgress = 0f;
    private float _foamProgress = 0f;
    private float _stirProgress = 0f;

    private MilkJugDepth _currentJugDepth = MilkJugDepth.NONE;

    private InputSystem_Actions.FrothingActions _frothingActions;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _nozzle.OnJugDepthChange.AddListener((newDepth) => _currentJugDepth = newDepth);

        _frothingActions = ControlsManager.FrothingActions;
        _frothingActions.Complete.performed += _ => HandleCompleted();

        Events.MiniGameEvents.OnFrothingStart.AddListener(HandleStart);
        Events.MiniGameEvents.OnFrothingEnd.AddListener(HandleEnd);

        _canvasGroup.alpha = 0;
        _canvasGroup.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_currentJugDepth == MilkJugDepth.NONE)
            return;

        if (_currentJugDepth == MilkJugDepth.SHALLOW && _foamProgress < 1)
        {
            _foamProgress += Time.deltaTime * _foamGainSpeed;
            _foamProgressBar.fillAmount = _foamProgress;
        }

        if (_currentJugDepth == MilkJugDepth.DEEP && _stirProgress < _foamProgress)
        {
            _stirProgress += Time.deltaTime * _stirGainSpeed;
            _stirProgressBar.fillAmount = _stirProgress;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Start Mini-Game")]
    private void DEBUG_StartFrothing()
    {
        Events.MiniGameEvents.OnFrothingStart.Invoke();
    }
#endif

    private void ResetProgress()
    {
        _foamProgress = 0f;
        _foamProgressBar.fillAmount = _foamProgress;

        _stirProgress = 0f;
        _stirProgressBar.fillAmount = _stirProgress;
    }

    private bool IsInRange(float value, float min, float max)
    {
        return value < max && value > min;
    }

    private int CalculatePointsDeduction(float value, float min, float max, float step)
    {
        if (IsInRange(value, min, max))
            return 0;

        float diff = value < min ? min - value : value - max;
        return Mathf.RoundToInt((diff / step) * 10);
    }

    private void HandleStart()
    {
        ResetProgress();

        _canvasGroup.gameObject.SetActive(true);
        Tween.Alpha(_canvasGroup, 1f, 0.5f);

        _frothingActions.Enable();
    }

    private async void HandleEnd(int _)
    {
        await Tween.Alpha(_canvasGroup, 0f, 0.5f);

        _canvasGroup.gameObject.SetActive(false);
    }

    private async void HandleCompleted()
    {
        _frothingActions.Disable();

        var qualityDeduction = 0;

        qualityDeduction += CalculatePointsDeduction(
            _foamProgress,
            _foamPerfectMinProgress,
            _foamPerfectMaxProgress,
            0.1f
        );

        qualityDeduction += CalculatePointsDeduction(
            _stirProgress,
            _foamPerfectMinProgress,
            _foamPerfectMaxProgress,
            0.1f
        );

        Events.MiniGameEvents.OnFrothingEnd.Invoke(qualityDeduction);

        await Tween.Delay(2f);

        Events.MiniGameEvents.OnFrothingTransitionOut.Invoke();
    }
}
