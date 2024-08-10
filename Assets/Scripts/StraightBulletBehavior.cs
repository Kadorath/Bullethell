using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBulletBehavior : BulletBehavior
{
    public Vector2 direction;

    void FixedUpdate()
    {
        if(delay_time >= 0f) {
            delay_time -= Time.fixedDeltaTime;
            transform.localScale = new Vector3(Mathf.Lerp(final_scale_x,final_scale_y*3,delay_time/indicate_time)
                                                ,Mathf.Lerp(final_scale_y,final_scale_y*3,delay_time/indicate_time),
                                                1f);
        }
        else {
            transform.Translate(direction*speed*Time.deltaTime, Space.World);
            if (spin) {
                transform.Rotate(new Vector3(0f,0f,3f));
            }

            if(grazing) {
                GetComponent<SpriteRenderer>().color = graze_color;
            }
            else {
                GetComponent<SpriteRenderer>().color = default_color;
            }
        }
    }
}
