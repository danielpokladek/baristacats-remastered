#nullable enable

using UnityEngine;
using UnityEngine.UI;

public class OrderTicketUI : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField]
    private GameObject _beanIcon = null!;

    [SerializeField]
    private GameObject _milkIcon = null!;

    [SerializeField]
    private Image _milkSprite = null!;

    [SerializeField]
    private Image _timerBar = null!;

    [SerializeField]
    private Sprite _unknownOrderSprite = null!;

    private float _maxWaitTime;
    private float _currentWaitTime;
    private CoffeeData? _coffeeData;

    private void Awake()
    {
        enabled = false;

        _beanIcon.SetActive(false);
        _milkIcon.SetActive(false);
        _timerBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        _currentWaitTime -= Time.deltaTime;

        _timerBar.fillAmount = _currentWaitTime / _maxWaitTime;

        if (_currentWaitTime <= 0)
        {
            enabled = false;
        }
    }

    public void Init(OrderData orderData)
    {
        _coffeeData = orderData.CoffeeData;

        _maxWaitTime = orderData.MaxWaitTime;
        _currentWaitTime = orderData.MaxWaitTime;

        _timerBar.fillAmount = 1;
        _timerBar.gameObject.SetActive(true);

        _milkSprite.sprite = _unknownOrderSprite;
        _milkSprite.gameObject.SetActive(true);
    }

    public void RevealOrder()
    {
        _beanIcon.SetActive(true);

        if (_coffeeData == null)
        {
            Debug.LogError("Tried to show order on ticket but no order set!", gameObject);
            return;
        }

        if (_coffeeData.Milk == MilkType.NONE)
        {
            _milkIcon.SetActive(false);
            return;
        }

        var milkType = _coffeeData.Milk;
        var milkEmote = CharacterEmotes.Instance.GetMilkEmote(milkType);

        _milkSprite.sprite = milkEmote;
        _milkIcon.SetActive(true);
    }

    public void StartTimer()
    {
        enabled = true;
    }

    public void StopTimer()
    {
        enabled = false;
    }

    public void Reset()
    {
        enabled = false;

        _beanIcon.gameObject.SetActive(false);
        _milkIcon.gameObject.SetActive(false);

        _timerBar.fillAmount = 1;
    }
}
