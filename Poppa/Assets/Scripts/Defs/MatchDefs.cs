///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MatchDefs
{
    public const int k_minimumGroupSize = 3;
    
    public enum MatchType
    {
        None = -1,
        Blue,
        Cyan,
        Green,
        Magenta,
        Red,
        Yellow,
        NormalCount,
        Omni = NormalCount,
        Bomb,
        Count
    }

    private static readonly Dictionary<MatchType, Color> m_typeColours = new Dictionary<MatchType, Color>()
    {
        {MatchType.Blue, Color.blue},
        {MatchType.Cyan, Color.cyan},
        {MatchType.Green, Color.green},
        {MatchType.Magenta, Color.magenta},
        {MatchType.Red, Color.red},
        {MatchType.Yellow, Color.yellow},
        {MatchType.Omni, Color.white},
        {MatchType.Bomb, Color.gray},
    };

    public static MatchType RandomMatchType => (MatchType)Mathf.FloorToInt(Random.value * Convert.ToInt32(MatchType.NormalCount));
    public static MatchType RandomSpecialMatchType => Convert.ToInt32(MatchType.NormalCount) + (MatchType)Mathf.FloorToInt(Random.value * (Convert.ToInt32(MatchType.Count) - Convert.ToInt32(MatchType.NormalCount)));

    public static Color ColorFromType(MatchType type)
    {
        return m_typeColours[type];
    }
}