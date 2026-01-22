using UnityEngine.Events;

public class FrothingEvents
{
    public readonly UnityEvent OnFrothingStart = new();
    public readonly UnityEvent<int> OnFrothingEnd = new();
}

public static class Events
{
    public static readonly FrothingEvents FrothingEvents = new();
}
