/*
 * Game objects with this script can be harvested for the resource(s) that it holds
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
    [SerializeField]
    private Resource resource;   
    [SerializeField]
    private float amount;

    // Update is called once per frame
    void Update()
    {
        if (amount < 0)
            Destroy(gameObject);
    }

    public void Mine(float amount)
    {
        this.amount -= amount;
    }

    public Resource GetResource()
    {
        return resource;
    }
}
