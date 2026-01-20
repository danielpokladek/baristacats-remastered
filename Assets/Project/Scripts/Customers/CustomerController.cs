using UnityEngine;

[RequireComponent(typeof(CustomerMovement))]
public class CustomerController : MonoBehaviour
{
    public CoffeeData CoffeeData { get; private set; }
    public CustomerMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<CustomerMovement>();
    }
}
