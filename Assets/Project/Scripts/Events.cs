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
    public readonly UnityEvent<CustomerController> OnOrderFailed = new();
    public readonly UnityEvent<CustomerController> OnOrderSuccessful = new();
    public readonly UnityEvent<CustomerController> OnOutOfTime = new();
}

public static class Events
{
    public static readonly MiniGameEvents MiniGameEvents = new();
    public static readonly CustomerEvents CustomerEvents = new();

    public static readonly UnityEvent<float> OnSanityChange = new();

    public static readonly UnityEvent OnGameStart = new();
    public static readonly UnityEvent OnGameOver = new();

    public static readonly UnityEvent RushStart = new();
    public static readonly UnityEvent RushEnd = new();
}
