using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour
{
    public const float HO_BOUND = 8f;
    public const float VO_BOUND = 8f;
    public GameObject enemy;
    public GameObject player;
    public int stage;


    void Start()
    {
        stage = 0;
    }

    void Update()
    {
        if (Input.GetKey("escape")) {
            Application.Quit();
        }
    }

    public GameObject[] CreatePool(GameObject bul, int n) {
        GameObject[] new_pool = new GameObject[n];
        for (int i = 0; i < n; i ++) {
            new_pool[i] = Instantiate(bul, transform);
            new_pool[i].SetActive(false);
            new_pool[i].name = bul.name;
        }
        return new_pool;
    }

    void FixedUpdate() {
        // Boundary Detection
        foreach (Collider2D bul in Physics2D.OverlapBoxAll(new Vector2(HO_BOUND,0f),new Vector2(1f,14f),0f,
            LayerMask.GetMask("EnemyBullets","PlayerBullets"))) {
                BulletBehavior bh = bul.GetComponent<BulletBehavior>();
                if (!bh.reserve && bul.enabled) {
                    bh.DestroySelf();
                }
            }
        foreach (Collider2D bul in Physics2D.OverlapBoxAll(new Vector2(-HO_BOUND,0f),new Vector2(1f,14f),0f,
            LayerMask.GetMask("EnemyBullets","PlayerBullets"))) {
                BulletBehavior bh = bul.GetComponent<BulletBehavior>();
                if (!bh.reserve && bul.enabled) {
                    bh.DestroySelf();
                }
            }
        foreach (Collider2D bul in Physics2D.OverlapBoxAll(new Vector2(0f,VO_BOUND),new Vector2(14f,1f),0f,
            LayerMask.GetMask("EnemyBullets","PlayerBullets"))) {
                BulletBehavior bh = bul.GetComponent<BulletBehavior>();
                if (!bh.reserve && bul.enabled) {
                    bh.DestroySelf();
                }
            }
        foreach (Collider2D bul in Physics2D.OverlapBoxAll(new Vector2(0f,-VO_BOUND),new Vector2(14f,1f),0f,
            LayerMask.GetMask("EnemyBullets","PlayerBullets"))) {
                BulletBehavior bh = bul.GetComponent<BulletBehavior>();
                if (!bh.reserve && bul.enabled) {
                    bh.DestroySelf();
                }
            }
    }
}
