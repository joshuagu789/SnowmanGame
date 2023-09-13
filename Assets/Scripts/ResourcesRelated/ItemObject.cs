using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Consumable,
    Equipment,
    Core    // Item snowmen drop on death that allows for their ressurection
}

public class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;
    public float volume;
}
