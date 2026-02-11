#nullable enable

using PrimeTween;
using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed;

    private Tween? _currentMoveTween;

    public Tween MoveTo(Vector2 destination)
    {
        _currentMoveTween?.Stop();

        var distance = Vector2.Distance(transform.position, destination);
        var duration = distance / _movementSpeed;

        var moveTween = Tween
            .Position(transform, destination, duration, ease: Ease.Linear)
            .OnComplete(() => _currentMoveTween = null);

        _currentMoveTween = moveTween;

        return moveTween;
    }
}
