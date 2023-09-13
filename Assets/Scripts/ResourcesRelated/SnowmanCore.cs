/*
 * Snowmen drop cores on death that can be later used for their revival (such as by the player)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Core", menuName = "Inventory System/Items/Cores")]
public class SnowmanCore : ItemObject
{
    [SerializeField]
    private GameObject snowmanPrefab;   // Snowman's original form before becoming a core

    void Awake()
    {
        type = ItemType.Core;
    }
}
