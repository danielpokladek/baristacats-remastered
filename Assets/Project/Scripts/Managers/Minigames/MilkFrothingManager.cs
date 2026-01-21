using UnityEngine;
using UnityEngine.UI;

public class MilkFrothingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FrotherNozzle _nozzle;

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

    [Header("Temperature")]
    [SerializeField]
    private Image _temperatureGauge;

    [SerializeField]
    private Color _coldColor;

    [SerializeField]
    private Color _perfectColor;

    [SerializeField]
    private Color _hotColor;

    [SerializeField]
    private float _temperatureGainSpeed = 0.2f;

    [SerializeField]
    private float _temperaturePerfectMinProgress = 0.66f;

    [SerializeField]
    private float _temperaturePerfectMaxProgress = 0.77f;

    private float _temperatureProgress = 0f;
    private float _foamProgress = 0f;
    private float _stirProgress = 0f;

    private MilkJugDepth _currentJugDepth = MilkJugDepth.NONE;

    private void Start()
    {
        _temperatureProgress = 0.1f;
        _temperatureGauge.fillAmount = _temperatureProgress;

        _foamProgress = 0f;
        _foamProgressBar.fillAmount = _foamProgress;

        _stirProgress = 0f;
        _stirProgressBar.fillAmount = _stirProgress;

        _nozzle.OnJugDepthChange.AddListener(
            (newDepth) =>
            {
                Debug.Log($"Nozzle depth change: {newDepth}");
                _currentJugDepth = newDepth;
            }
        );

        Cursor.lockState = CursorLockMode.Locked;

        var actions = ControlsManager.FrothingActions;

        actions.Complete.performed += _ =>
        {
            var deductions = 0f;

            deductions += CalculatePointsDeduction(
                _temperatureProgress,
                _temperaturePerfectMinProgress,
                _temperaturePerfectMaxProgress,
                0.1f
            );

            deductions += CalculatePointsDeduction(
                _foamProgress,
                _foamPerfectMinProgress,
                _foamPerfectMaxProgress,
                0.1f
            );

            deductions += CalculatePointsDeduction(
                _stirProgress,
                _foamPerfectMinProgress,
                _foamPerfectMaxProgress,
                0.1f
            );
        };

        actions.Enable();
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

        if (_temperatureProgress < 1)
        {
            _temperatureProgress += Time.deltaTime * _temperatureGainSpeed;

            _temperatureGauge.fillAmount = _temperatureProgress;
            _temperatureGauge.color = Color.Lerp(_coldColor, _hotColor, _temperatureProgress);
        }
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
}
