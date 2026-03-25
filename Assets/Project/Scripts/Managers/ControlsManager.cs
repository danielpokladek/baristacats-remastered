public static class ControlsManager
{
    private static readonly InputSystem_Actions _inputActions = new();

    public static InputSystem_Actions.PlayerActions PlayerActions => _inputActions.Player;
    public static InputSystem_Actions.FrothingActions FrothingActions => _inputActions.Frothing;
    public static InputSystem_Actions.MilkPickingActions MilkPickingActions =>
        _inputActions.MilkPicking;

    public static void EnablePlayerControls() => _inputActions.Player.Enable();

    public static void DisablePlayerControls() => _inputActions.Player.Disable();

    public static void EnableFrothingControls() => _inputActions.Frothing.Enable();

    public static void DisableFrothingControls() => _inputActions.Frothing.Disable();
}
