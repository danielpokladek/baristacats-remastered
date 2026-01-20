using UnityEngine;
using UnityEngine.UI;

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
    }
}
