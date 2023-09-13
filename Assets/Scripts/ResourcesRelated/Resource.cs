/*
 * Resources serve as both currency and building materials that the player can use
 *  - Some entities can build with resources
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Inventory System/Items/Resource")]
public class Resource : ItemObject
{
    void Awake()
    {
        type = ItemType.Resource;
    }
}
