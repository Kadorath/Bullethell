using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBehavior : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy") || other.CompareTag("Bullet") || other.CompareTag("Laser")) {
            transform.parent.GetComponent<PlayerBehavior>().PlayerDie();
        }
    }
}
