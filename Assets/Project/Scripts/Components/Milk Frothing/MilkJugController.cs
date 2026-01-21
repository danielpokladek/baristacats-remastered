using UnityEngine;

public class MilkJugController : MonoBehaviour
{
    [SerializeField]
    private Vector2 _minPos;

    [SerializeField]
    private Vector2 _maxPos;

    private readonly float _mouseSensitivity = 0.5f;

    void Start()
    {
        var actions = ControlsManager.FrothingActions;

        actions.Move.performed += ctx =>
        {
            var delta = ctx.ReadValue<Vector2>();
            var pos = transform.position;

            var newPos = new Vector3(
                pos.x + delta.x * (Time.deltaTime * _mouseSensitivity),
                pos.y + delta.y * (Time.deltaTime * _mouseSensitivity),
                pos.z
            );

            newPos.x = Mathf.Clamp(newPos.x, _minPos.x, _maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, _minPos.y, _maxPos.y);

            transform.transform.position = newPos;
        };
    }
}
