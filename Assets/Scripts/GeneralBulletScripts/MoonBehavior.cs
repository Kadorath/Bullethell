using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonBehavior : BulletBehavior
{
    void OnTriggerEnter2D(Collider2D other) {
        if (other.name == "bul circle") {
            other.GetComponent<StraightBulletBehavior>().DestroySelf();
        }
    }
}
