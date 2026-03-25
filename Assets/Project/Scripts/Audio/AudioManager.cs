using PrimeTween;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField]
    AudioClip[] _backgroundMusic;

    [SerializeField]
    AudioClip[] _rushMusic;

    [Header("Ambience")]
    [SerializeField]
    AudioClip[] _ambience;

    [Header("References")]
    [SerializeField]
    AudioSource _backgroundMusicSource;

    [SerializeField]
    AudioSource _rushMusicSource;

    [SerializeField]
    AudioSource _rushAlarmSource;

    [SerializeField]
    AudioSource _ambientSource;

    private float _volume = 0.3f;

    private void Start()
    {
        SetupSources();

        var bgm = GetBackgroundClip();

        _backgroundMusicSource.clip = bgm;
        _backgroundMusicSource.Play();

        Events.RushStart.AddListener(TransitionToRush);
        Events.RushEnd.AddListener(TransitionToNormal);

        Events.OnGameOver.AddListener(HandleGameOver);
    }

    private void SetupSources()
    {
        _backgroundMusicSource.volume = _volume;
        _backgroundMusicSource.loop = true;
        _backgroundMusicSource.playOnAwake = false;

        _rushMusicSource.volume = 0f;
        _rushMusicSource.loop = true;
        _rushMusicSource.playOnAwake = false;

        _ambientSource.volume = _volume;
        _ambientSource.loop = true;
        _ambientSource.playOnAwake = false;
    }

    private void TransitionToRush()
    {
        var transitionDuration = 0.25f;

        var rushClip = GetRushClip();

        _rushMusicSource.clip = rushClip;
        _rushAlarmSource.Play();

        Sequence
            .Create()
            .Chain(Tween.Delay(2f - transitionDuration))
            .Group(Tween.AudioVolume(_backgroundMusicSource, 0f, duration: transitionDuration))
            .Group(Tween.AudioVolume(_ambientSource, 0f, duration: transitionDuration))
            .Chain(Tween.Delay(0.5f))
            .ChainCallback(() => _rushMusicSource.Play())
            .Group(Tween.AudioVolume(_rushMusicSource, _volume, duration: transitionDuration));
    }

    private void TransitionToNormal()
    {
        var transitionDuration = 0.25f;

        var background = GetBackgroundClip();
        var ambience = GetAmbientClip();

        _backgroundMusicSource.clip = background;
        _ambientSource.clip = ambience;

        Sequence
            .Create()
            .Group(Tween.AudioVolume(_rushMusicSource, 0f, duration: transitionDuration))
            .Chain(Tween.Delay(0.5f))
            .ChainCallback(() =>
            {
                _backgroundMusicSource.Play();
                _ambientSource.Play();
            })
            .Group(Tween.AudioVolume(_backgroundMusicSource, _volume, duration: transitionDuration))
            .Group(Tween.AudioVolume(_ambientSource, _volume, duration: transitionDuration));
    }

    private AudioClip GetBackgroundClip()
    {
        return _backgroundMusic[Random.Range(0, _backgroundMusic.Length)];
    }

    private AudioClip GetRushClip()
    {
        return _rushMusic[Random.Range(0, _rushMusic.Length)];
    }

    private AudioClip GetAmbientClip()
    {
        return _ambience[Random.Range(0, _ambience.Length)];
    }

    private void HandleGameOver()
    {
        _ambientSource.Stop();
        _rushAlarmSource.Stop();
        _rushMusicSource.Stop();
        _backgroundMusicSource.Stop();
    }
}
