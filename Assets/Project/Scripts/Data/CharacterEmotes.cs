using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MilkEmote
{
    public MilkType MilkType;
    public Sprite MilkImage;
}

public class CharacterEmotes : MonoBehaviour
{
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

    private void Start()
    {
        _milkEmotesDictionary = new();

        foreach (var data in _milkEmotes)
        {
            _milkEmotesDictionary.Add(data.MilkType, data.MilkImage);
        }
    }
}
