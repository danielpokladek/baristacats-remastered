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
        if (Vector2.Distance(transform.position, destination) < 0.1)
            // Returning Tween? makes it so you can't await it, even after checking it isn't null.
            // TODO: Find a better solution for this, but for now it will do.
            return Tween.Delay(0);

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
