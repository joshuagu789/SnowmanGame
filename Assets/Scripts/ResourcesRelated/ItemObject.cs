using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Consumable,
    Equipment,
}

public class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;
    public float volume;
}
