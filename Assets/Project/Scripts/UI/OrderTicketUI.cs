#nullable enable

using System.Collections.Generic;
using PrimeTween;
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
    private Image _patienceBar = null!;

    [SerializeField]
    private Sprite _unknownOrderSprite = null!;

    private CoffeeData? _coffeeData;
    private Tween? _currentTween;

    private void Awake()
    {
        _beanIcon.SetActive(false);
        _milkIcon.SetActive(false);
        _patienceBar.gameObject.SetActive(false);
    }

    public void Init(OrderData orderData)
    {
        _coffeeData = orderData.CoffeeData;

        _patienceBar.fillAmount = 1;
        _patienceBar.gameObject.SetActive(true);

        _milkSprite.sprite = _unknownOrderSprite;
        _milkSprite.gameObject.SetActive(true);
    }

    public void SetPatienceBarFill(float fillAmount)
    {
        _patienceBar.fillAmount = fillAmount;
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

    public Tween MoveTo(Vector3 destination)
    {
        _currentTween?.Stop();

        var tween = Tween.LocalPosition(transform, destination, duration: 0.3f);
        _currentTween = tween;

        return tween;
    }

    public void Reset()
    {
        enabled = false;

        _beanIcon.gameObject.SetActive(false);
        _milkIcon.gameObject.SetActive(false);

        _patienceBar.fillAmount = 1;
    }
}
