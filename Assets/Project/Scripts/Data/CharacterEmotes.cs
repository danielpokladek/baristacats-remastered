using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MilkEmote
{
    public MilkType MilkType;
    public Sprite MilkImage;
}

public class CharacterEmotes : MonoBehaviour
{
    public static CharacterEmotes Instance { get; private set; }

    [SerializeField]
    private Sprite _blankEmote;

    [SerializeField]
    private Sprite _coffeeCupEmote;

    [SerializeField]
    private MilkEmote[] _milkEmotes;

    [SerializeField]
    private Sprite[] _sadEmotes;

    [SerializeField]
    private Sprite[] _happyEmotes;

    private Dictionary<MilkType, Sprite> _milkEmotesDictionary;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of `CharacterEmotes` found in scene!");
            return;
        }
        else
        {
            Instance = this;
        }

        _milkEmotesDictionary = new();

        foreach (var data in _milkEmotes)
        {
            _milkEmotesDictionary.Add(data.MilkType, data.MilkImage);
        }
    }

    public Sprite PlainEmote => _blankEmote;
    public Sprite PlainCoffeeEmote => _coffeeCupEmote;

    public Sprite GetHappyEmote()
    {
        var index = Random.Range(0, _happyEmotes.Length);

        return _happyEmotes[index];
    }

    public Sprite GetSadEmote()
    {
        var index = Random.Range(0, _sadEmotes.Length);

        return _sadEmotes[index];
    }
}
