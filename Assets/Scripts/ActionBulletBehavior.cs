using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBulletBehavior : BulletBehavior
{
    public Vector2 destination;
    public delegate void AtDestination(GameObject self);
    public AtDestination at_destination;

    void FixedUpdate()
    {
        // Move towards destination, and if bullet just moved to destination, call at_destination
        if (Vector2.Distance(transform.position, destination) >= 0.001f) {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed*Time.fixedDeltaTime);

            if (at_destination != null && !(Vector2.Distance(transform.position, destination) >= 0.001f)) {
                at_destination(gameObject);
            }
        }
    }
}
