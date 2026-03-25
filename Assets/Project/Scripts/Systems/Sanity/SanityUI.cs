#nullable enable
using PrimeTween;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanityUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    SpriteRenderer _sanityGlow;

    [SerializeField]
    Volume _volume;

    [Header("Colors")]
    [SerializeField]
    Color _fullSanityColor;

    [SerializeField]
    Color _lowSanityColor;

    [Header("Vignette")]
    [SerializeField]
    float _minVignette = 0;

    [SerializeField]
    float _maxVignette = 0.4f;

    private Tween? _vignetteTween;

    private void Start()
    {
        Events.OnSanityChange.AddListener(HandleSanityChange);
    }

    private void HandleSanityChange(float normalizedProgress)
    {
        _sanityGlow.color = Color.Lerp(_fullSanityColor, _lowSanityColor, normalizedProgress);

        if (_volume.profile.TryGet<Vignette>(out var vignette))
        {
            _vignetteTween?.Stop();

            var newVignette = Mathf.Lerp(_minVignette, _maxVignette, normalizedProgress);

            _vignetteTween = Tween.Custom(
                vignette.intensity.value,
                newVignette,
                0.5f,
                (val) => vignette.intensity.value = val
            );
        }
    }
}
