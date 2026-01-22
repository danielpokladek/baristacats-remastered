using UnityEngine.Events;

public class MiniGameEvents
{
    public readonly UnityEvent OnFrothingTransitionIn = new();
    public readonly UnityEvent OnFrothingStart = new();
    public readonly UnityEvent<int> OnFrothingEnd = new();
    public readonly UnityEvent OnFrothingTransitionOut = new();
}

public static class Events
{
    public static readonly MiniGameEvents MiniGameEvents = new();
}
