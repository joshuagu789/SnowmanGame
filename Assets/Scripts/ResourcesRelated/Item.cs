using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemObject item;
    [SerializeField]
    private int amount;

    public ItemObject GetItem() { return item; }

    public int GetAmount() { return amount; }

    public void SetAmount(int amount)
    {
        this.amount = amount;
    }
}
