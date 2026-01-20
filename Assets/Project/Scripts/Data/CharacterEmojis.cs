using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MilkEmoji
{
    public MilkType MilkType;
    public Sprite MilkImage;
}

public class CharacterEmojis : MonoBehaviour
{
    [SerializeField]
    private Sprite _blankEmoji;

    [SerializeField]
    private Sprite _coffeeCupEmoji;

    [SerializeField]
    private MilkEmoji[] _milkEmojis;

    [SerializeField]
    private Sprite[] _sadEmojis;

    [SerializeField]
    private Sprite[] _happyEmoji;

    private Dictionary<MilkType, Sprite> _milkEmojiDictionary;

    private void Start()
    {
        _milkEmojiDictionary = new();

        foreach (var data in _milkEmojis)
        {
            _milkEmojiDictionary.Add(data.MilkType, data.MilkImage);
        }
    }
}
