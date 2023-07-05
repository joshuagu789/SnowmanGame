using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLock : MonoBehaviour
{
    public IceBeamRig iceBeamScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        //transform.localScale = iceBeamScript.hit.collider.transform.localScale;
        //Vector3 localScale = iceBeamScript.hit.collider.gameObject.transform.localScale;
        //transform.localScale = localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
