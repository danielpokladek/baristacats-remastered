using UnityEngine;
using UnityEngine.UI;

public class SanityUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SpriteRenderer _sanityGlow;

    [Header("Colors")]
    [SerializeField]
    private Color _fullSanityColor;

    [SerializeField]
    private Color _lowSanityColor;

    private float _maxSanity = 1f;
    private float _currentSanity = 1f;

    public void UpdateSanityUI(float normalizedProgress)
    {
        _sanityGlow.color = Color.Lerp(_fullSanityColor, _lowSanityColor, normalizedProgress);
    }
}
