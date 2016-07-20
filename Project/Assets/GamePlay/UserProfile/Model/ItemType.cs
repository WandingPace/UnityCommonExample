using UnityEngine;
using System;
using System.Collections;
// 
public enum ItemType :int
{
    Unknown = 0,
    Gold,
    Experience,
    Energy,
    Gem,
    OnlineEnergy,
    Box,
};

public class ItemIdHeaders
{
    public const string Unknown = "Unknown";
    public const string Gold = "SC";
    public const string Experience = "Exp";
    public const string Energy = "AP";
    public const string Gem = "HC";
    public const string OnlineEnergy = "OE";
    public const string Box = "Bx";
};

public class ItemTypeUtilty
{
    public static ItemType GetType(string itemId) 
    {
        if (itemId.StartsWith(ItemIdHeaders.Gold, StringComparison.OrdinalIgnoreCase))
            return ItemType.Gold;
        if (itemId.StartsWith(ItemIdHeaders.Experience, StringComparison.OrdinalIgnoreCase))
            return ItemType.Experience;
        if (itemId.StartsWith(ItemIdHeaders.Energy, StringComparison.OrdinalIgnoreCase))
            return ItemType.Energy;
        if (itemId.StartsWith(ItemIdHeaders.Gem, StringComparison.OrdinalIgnoreCase))
            return ItemType.Gem;
        if (itemId.StartsWith(ItemIdHeaders.OnlineEnergy, StringComparison.OrdinalIgnoreCase))
            return ItemType.OnlineEnergy;
        if (itemId.StartsWith(ItemIdHeaders.Box, StringComparison.OrdinalIgnoreCase))
            return ItemType.Box;
        return ItemType.Unknown;
    }
}
