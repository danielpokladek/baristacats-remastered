using UnityEngine;

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

    public void UpdateSanityUI(float normalizedProgress)
    {
        _sanityGlow.color = Color.Lerp(_fullSanityColor, _lowSanityColor, normalizedProgress);
    }
}
