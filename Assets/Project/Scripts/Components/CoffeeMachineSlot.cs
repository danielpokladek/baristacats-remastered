using System.Threading.Tasks;
using PrimeTween;
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

    private AudioSource _audioSource;

    public bool IsBrewing { get; private set; }
    public bool IsFree { get; private set; }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        Reset();
    }

    public async Task StartBrewing()
    {
        IsFree = false;
        IsBrewing = true;

        _progressImage.fillAmount = 0;
        _progressImage.enabled = true;

        gameObject.SetActive(true);

        _audioSource.PlayOneShot(_brewingSound);

        await Tween.Custom(
            0f,
            BrewDuration,
            BrewDuration,
            (val) => _progressImage.fillAmount = val / BrewDuration,
            Ease.Linear
        );

        CompleteBrewing();
    }

    public void Reset()
    {
        IsBrewing = false;
        IsFree = true;

        _completeImage.enabled = false;

        _progressImage.enabled = false;
        _progressImage.fillAmount = 0;

        gameObject.SetActive(false);
    }

    private void CompleteBrewing()
    {
        _progressImage.fillAmount = 0;

        _completeImage.sprite = _cupSprite;
        _completeImage.enabled = true;

        OnCoffeeBrewed.Invoke();

        IsBrewing = false;
        IsFree = false;
    }
}
