using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(CustomerMovement))]
public class CustomerController : MonoBehaviour
{
    [SerializeField]
    private Image _emoteImage;

    [SerializeField]
    private Image _coffeeImage;

    public CoffeeData CoffeeData { get; private set; }
    public CustomerMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<CustomerMovement>();

        _emoteImage.color = new(1, 1, 1, 0);
        _coffeeImage.color = new(1, 1, 1, 0);
    }

    public async Task ShowOrder()
    {
        _emoteImage.sprite = CharacterEmotes.Instance.PlainEmote;
        _coffeeImage.sprite = CharacterEmotes.Instance.PlainCoffeeEmote;

        await Sequence
            .Create()
            .Group(Tween.Alpha(_emoteImage, 1f, 0.5f))
            .Group(Tween.Alpha(_coffeeImage, 1f, 0.5f));

        await Tween.Delay(1.5f);

        await Sequence
            .Create()
            .Group(Tween.Alpha(_emoteImage, 0f, 0.5f))
            .Group(Tween.Alpha(_coffeeImage, 0f, 0.5f));
    }
}
