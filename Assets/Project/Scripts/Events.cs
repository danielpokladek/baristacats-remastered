using UnityEngine.Events;

public class MiniGameEvents
{
    public readonly UnityEvent OnFrothingTransitionIn = new();
    public readonly UnityEvent OnFrothingStart = new();
    public readonly UnityEvent<int> OnFrothingEnd = new();
    public readonly UnityEvent OnFrothingTransitionOut = new();
}

public class CustomerEvents
{
    public readonly UnityEvent<CustomerController> OrderFailed = new();
    public readonly UnityEvent<CustomerController> OrderSuccessful = new();
}

public static class Events
{
    public static readonly MiniGameEvents MiniGameEvents = new();
    public static readonly CustomerEvents CustomerEvents = new();

    public static readonly UnityEvent<int> OnDifficultyLevelChange = new();
}
