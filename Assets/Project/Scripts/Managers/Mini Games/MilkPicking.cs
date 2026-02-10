using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MilkPicking : MonoBehaviour
{
    [SerializeField]
    private Image[] _milks;

    private int _currentIndex;
    private int _milksLength;

    private void Start()
    {
        _milksLength = _milks.Length;
        _currentIndex = 0;

        SelectMilk(true);
    }

    private void Awake()
    {
        var milkPickingActions = ControlsManager.MilkPickingActions;

        milkPickingActions.Previous.performed += _ => HandleSelectionChange(-1);
        milkPickingActions.Next.performed += _ => HandleSelectionChange(1);
        milkPickingActions.Confirm.performed += _ => HandleSelectionConfirm();
    }

    private void HandleSelectionChange(int delta)
    {
        _currentIndex = (_currentIndex + delta) % _milksLength;
        SelectMilk();
    }

    private void SelectMilk(bool isInstant = false)
    {
        for (int i = 0; i < _milksLength; i++)
        {
            var milk = _milks[i];
            var alpha = i == _currentIndex ? 1f : .5f;
            var duration = isInstant ? 0f : 0.15f;

            milk.CrossFadeAlpha(alpha, duration, true);
        }
    }

    private void HandleSelectionConfirm()
    {
        var milkPicked = GetMilkForIndex();
        MiniGameManager.Instance.HandleMilkPicked(milkPicked);
    }

    private void Reset()
    {
        _currentIndex = 0;
        SelectMilk(true);
    }

    private MilkType GetMilkForIndex()
    {
        switch (_currentIndex)
        {
            case 0:
                return MilkType.BLUE;

            case 1:
                return MilkType.RED;

            case 2:
                return MilkType.YELLOW;

            case 3:
                return MilkType.CYAN;
        }

        throw new System.Exception("Invalid milk type picked!");
    }
}
