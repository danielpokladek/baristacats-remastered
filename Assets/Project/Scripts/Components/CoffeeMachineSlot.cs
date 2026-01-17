using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachineSlot : MonoBehaviour
{
    [HideInInspector]
    public float BrewDuration;

    [SerializeField]
    private Image _progressImage;

    [SerializeField]
    private Sprite _cupSprite;

    [SerializeField]
    private Sprite _frotherSprite;

    [SerializeField]
    private Image _completeImage;

    private float _brewTimer;
    private bool _isBrewing;

    public bool IsBrewing
    {
        get { return _isBrewing; }
        set
        {
            _brewTimer = 0;
            _isBrewing = value;
        }
    }
    public bool IsFree { get; set; }

    private void Start()
    {
        IsBrewing = false;
        IsFree = true;

        _completeImage.enabled = false;

        _progressImage.fillAmount = 0;
        _brewTimer = 0;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsBrewing)
            return;

        _brewTimer += Time.deltaTime;
        _progressImage.fillAmount = _brewTimer / BrewDuration;

        if (_brewTimer >= BrewDuration)
        {
            CompleteBrewing();
        }
    }

    public void StartBrewing()
    {
        IsFree = false;
        IsBrewing = true;

        gameObject.SetActive(true);
    }

    private void CompleteBrewing()
    {
        IsBrewing = false;

        _progressImage.fillAmount = 0;

        _completeImage.sprite = _cupSprite;
        _completeImage.enabled = true;
    }
}
