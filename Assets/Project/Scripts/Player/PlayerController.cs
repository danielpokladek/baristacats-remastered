using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerInventory Inventory { get; private set; }

    private void Start()
    {
        Inventory = new PlayerInventory();

        ControlsManager.EnablePlayerControls();
    }
}
