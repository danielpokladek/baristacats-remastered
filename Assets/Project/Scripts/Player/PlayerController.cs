using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInventory Inventory { get; private set; }

    [SerializeField]
    private GameObject _handTransform;

    [SerializeField]
    private SpriteRenderer _itemInHand;

    [Header("Items")]
    [SerializeField]
    private Sprite _coffeeCup;

    [SerializeField]
    private Sprite _grinder;

    [SerializeField]
    private Sprite _milkRed;

    [SerializeField]
    private Sprite _milkBlue;

    [SerializeField]
    private Sprite _milkGreen;

    [SerializeField]
    private Sprite _milkYellow;

    private void Start()
    {
        Inventory = new PlayerInventory();

        ControlsManager.EnablePlayerControls();

        HandleInventoryUpdate();
        Inventory.OnInventoryUpdate.AddListener(HandleInventoryUpdate);
    }

    private void HandleInventoryUpdate()
    {
        if (!Inventory.IsHoldingItem)
        {
            _handTransform.SetActive(false);
            _itemInHand.sprite = null;
            return;
        }

        if (Inventory.IsHoldingBeans)
        {
            _handTransform.SetActive(true);
            _itemInHand.sprite = _grinder;
            return;
        }

        if (Inventory.CoffeeInHand != null)
        {
            _handTransform.SetActive(true);
            _itemInHand.sprite = _coffeeCup;
        }

        if (Inventory.MilkInHand != MilkType.NONE)
        {
            var milkSprite = GetMilkSprite(Inventory.MilkInHand);

            _handTransform.SetActive(true);
            _itemInHand.sprite = milkSprite;
        }
    }

    private Sprite GetMilkSprite(MilkType milk)
    {
        switch (milk)
        {
            case MilkType.BLUE:
                return _milkBlue;

            case MilkType.CYAN:
                return _milkGreen;

            case MilkType.RED:
                return _milkRed;

            case MilkType.YELLOW:
                return _milkYellow;

            case MilkType.NONE:
                Debug.LogError($"Invalid MilkType in hand: {milk}!");
                break;
        }

        throw new System.Exception($"Unable to retrieve milk for milk type: {milk}");
    }
}
