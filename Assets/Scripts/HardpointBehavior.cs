using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardpointBehavior : MonoBehaviour
{
    private float rotation_speed = 6f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotation_speed);        
    }
}
