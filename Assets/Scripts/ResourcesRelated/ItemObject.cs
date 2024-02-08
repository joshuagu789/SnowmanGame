using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    RESOURCE,
    CONSUMABLE,
    EQUIPMENT,
    CORE    // Item snowmen drop on death that allows for their ressurection
}

public class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;
    public float volume;
}
