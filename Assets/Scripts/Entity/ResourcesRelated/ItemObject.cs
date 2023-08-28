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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
