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
        if(delay_time >= 0f) {
            delay_time -= Time.fixedDeltaTime;
            transform.localScale = new Vector3(Mathf.Lerp(final_scale_x,final_scale_y*3,delay_time/indicate_time)
                                                ,Mathf.Lerp(final_scale_y,final_scale_y*3,delay_time/indicate_time),
                                                1f);
        }
        else {
            // Move towards destination, and if bullet just moved to destination, call at_destination
            if (Vector2.Distance(transform.localPosition, destination) >= 0.001f) {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, destination, speed*Time.fixedDeltaTime);

                if (at_destination != null && !(Vector2.Distance(transform.localPosition, destination) >= 0.001f)) {
                    at_destination(gameObject);
                }
            }
        }
    }
}
