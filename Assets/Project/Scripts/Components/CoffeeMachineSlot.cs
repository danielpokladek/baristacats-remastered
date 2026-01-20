using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoffeeMachineSlot : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnCoffeeBrewed;

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

    [Header("Audio")]
    [SerializeField]
    private AudioClip _brewingSound;

    private float _brewTimer;
    private bool _isBrewing;

    private AudioSource _audioSource;

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
    public bool IsDone { get; set; }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        Reset();
    }

    private void Update()
    {
        if (!IsBrewing)
            return;

        _brewTimer += Time.deltaTime;
        _progressImage.fillAmount = 1 - (_brewTimer / BrewDuration);

        if (_brewTimer >= BrewDuration)
        {
            CompleteBrewing();
        }
    }

    public void StartBrewing()
    {
        IsFree = false;
        IsBrewing = true;

        _progressImage.fillAmount = 1;
        _progressImage.enabled = true;

        gameObject.SetActive(true);
        enabled = true;

        _audioSource.PlayOneShot(_brewingSound);
    }

    public void Reset()
    {
        IsBrewing = false;
        IsFree = true;

        _completeImage.enabled = false;

        _progressImage.enabled = false;
        _progressImage.fillAmount = 1;

        _brewTimer = 0;

        gameObject.SetActive(false);
    }

    private void CompleteBrewing()
    {
        _progressImage.fillAmount = 0;

        _completeImage.sprite = _cupSprite;
        _completeImage.enabled = true;

        OnCoffeeBrewed.Invoke();

        IsBrewing = false;
        IsDone = true;

        enabled = false;
    }
}
