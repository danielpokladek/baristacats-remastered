using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MilkFrothingManager : MonoBehaviour
{
    public static UnityEvent<MilkJugDepth> OnJugDepthChange = new();

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

    private float _foamProgress = 0f;
    private float _stirProgress = 0f;

    private bool _isFrothing = false;

    private MilkJugDepth _currentJugDepth = MilkJugDepth.NONE;
    private InputSystem_Actions.FrothingActions _frothingActions;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        OnJugDepthChange.AddListener((newDepth) => _currentJugDepth = newDepth);

        _frothingActions = ControlsManager.FrothingActions;
        _frothingActions.Complete.performed += _ => HandleCompleted();

        _frothingActions.Froth.performed += _ => _isFrothing = true;
        _frothingActions.Froth.canceled += _ => _isFrothing = false;

        Events.MiniGameEvents.OnFrothingStart.AddListener(HandleStart);
        Events.MiniGameEvents.OnFrothingEnd.AddListener(HandleEnd);
    }

    private void Update()
    {
        if (_currentJugDepth == MilkJugDepth.NONE || !_isFrothing)
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

        _frothingActions.Enable();
    }

    private void HandleEnd(int _) { }

    private async void HandleCompleted()
    {
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

        MiniGameManager.Instance.HandleMilkFrothed(qualityDeduction);
    }
}
