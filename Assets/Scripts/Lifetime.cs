using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Sets a lifetime for a game object and destroys it once the timer runs out
 */

public class Lifetime : MonoBehaviour
{
    public float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
