using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemSO : ScriptableObject
{
    public string itemName;
    public enum itemGeneralType
    {
        hat,
        shirt,
        pant,
        shoe,
    }
    public Sprite icon;
}
