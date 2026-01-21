using UnityEngine;
using UnityEngine.Events;

public enum MilkJugDepth
{
    NONE,
    SHALLOW,
    DEEP,
}

public class FrotherNozzle : MonoBehaviour
{
    public UnityEvent<MilkJugDepth> OnJugDepthChange;

    private MilkJugDepth _currentJugDepth = MilkJugDepth.NONE;

    private void Update()
    {
        var collisions = Physics2D.OverlapCircleAll(transform.position, 0.02f);

        var newDepth = MilkJugDepth.NONE;

        foreach (var collision in collisions)
        {
            if (collision.CompareTag("Frothing Top"))
            {
                newDepth = MilkJugDepth.SHALLOW;
            }

            if (collision.CompareTag("Frothing Middle"))
            {
                newDepth = MilkJugDepth.DEEP;
            }
        }

        if (_currentJugDepth == newDepth)
            return;

        _currentJugDepth = newDepth;
        OnJugDepthChange.Invoke(_currentJugDepth);
    }
}
