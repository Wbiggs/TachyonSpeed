using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{

    public Transform target;
    public float snap;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(Mathf.Round(target.position.x / snap)*snap,0,MathF.Round(target.position.z/snap)*snap);
        target.position = pos;
    }
}
