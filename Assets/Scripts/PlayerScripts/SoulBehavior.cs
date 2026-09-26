using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBehavior : MonoBehaviour
{
    public Collider2D collided_bul;
    private PlayerBehavior _player;
    void Start()
    {
        _player = transform.parent.GetComponent<PlayerBehavior>();
    }
    void FixedUpdate() {
        // TODO: Check radius and adjust layers checked
        if (!_player.invulnerable)
        {
            collided_bul = Physics2D.OverlapCircle(
                transform.position,
                0.03f,
                LayerMask.GetMask("EnemyBullets", "Enemy")
            );
            if (collided_bul)
            {
                transform.parent.GetComponent<PlayerBehavior>().PlayerDie();
            }
        }
    }
}
