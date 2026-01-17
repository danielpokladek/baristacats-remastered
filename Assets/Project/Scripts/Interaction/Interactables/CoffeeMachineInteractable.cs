using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class CoffeeMachineInteractable : Interactable
{
    [SerializeField]
    private Image _leftCircleFill;

    [SerializeField]
    private Image _rightCircleFill;

    [SerializeField]
    private GameObject _leftCup;

    [SerializeField]
    private GameObject _centerCup;

    [SerializeField]
    private GameObject _rightCup;
}
