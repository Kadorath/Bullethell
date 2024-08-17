using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBehavior : MonoBehaviour
{
    public Collider2D collided_bul;

    void FixedUpdate() {
        // TODO: Check radius and adjust layers checked
        collided_bul = Physics2D.OverlapCircle(
            transform.position, 
            0.03f, 
            LayerMask.GetMask("EnemyBullets", "Enemy")
        );
        if (collided_bul) {
            transform.parent.GetComponent<PlayerBehavior>().PlayerDie();
        }
    }

    /*void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy") || other.CompareTag("Bullet") || other.CompareTag("Laser")) {
            transform.parent.GetComponent<PlayerBehavior>().PlayerDie();
        }
    }*/
}
