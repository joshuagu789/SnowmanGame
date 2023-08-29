/*
 * Entites with this script are able to hold items that they can use as consumables or materials
 *  - Player uses this as well as NPCs
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class StorageUnit : MonoBehaviour
{
    public List<StorageSlot> storage = new List<StorageSlot>();
    public float maxStorageSize;
    private float storageSize = 0;

    public void AddItem(ItemObject item, int amount)
    {
        bool hasItem = false;
        foreach (StorageSlot slot in storage)
        {
            if (slot.item == item)  // If entity already has item in a slot
            {
                hasItem = true;
                if(item.volume * amount + storageSize <= maxStorageSize)    // If entity has enough storage for it
                {
                    slot.AddAmount(amount);
                    storageSize += item.volume * amount;
                }
                break;
            }
        }
        if (!hasItem)
            storage.Add(new StorageSlot(item, amount));
    }
}

// One slot for each unique item/resource with its amount
[System.Serializable]
public class StorageSlot : ScriptableObject
{
    public ItemObject item;
    public float amount;
    public float slotVolume;

    public StorageSlot(ItemObject item, float amount)
    {
        this.item = item;
        this.amount = amount;
        slotVolume = item.volume * amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
        slotVolume += item.volume * value;
    }
}
*/
public class StorageUnit : MonoBehaviour
{
    private Dictionary<ItemObject, int> storage = new Dictionary<ItemObject, int>();
    [SerializeField]
    private float maxStorageSize;
    private float storageSize = 0;

    public void AddItem(ItemObject item, int amount)
    {
        if (storage.ContainsKey(item) && item.volume * amount + storageSize <= maxStorageSize)  // If storage already contains item
        {
            storage[item] += amount;
            storageSize += item.volume * amount;
        }
        else if (item.volume * amount + storageSize <= maxStorageSize)  // If storage doesn't have item and has room for it
        {
            storage.Add(item, amount);
            storageSize += item.volume * amount;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (ItemObject slot in storage.Keys)
            {
                print(slot.name + ": " + storage[slot]);
            }
        }
    }

    // Collecting items on contact
    private void OnCollisionEnter(Collision other)  
    {
        var item = other.collider.GetComponent<Item>();
        if (item != null)
        {
            GetComponent<StorageUnit>().AddItem(item.item, item.amount);
            Destroy(other.gameObject);
        }
    }
}