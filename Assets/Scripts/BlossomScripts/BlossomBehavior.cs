using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlossomBehavior : EnemyBehavior
{
    public GameObject bul_petal;
    public GameObject[] petal_pool;

    protected override void InitEnemy() {
        petal_pool = gameManager.CreatePool(bul_petal, 1000);
     
        patterns = new string[] {"Spell1"};
    }

    IEnumerator Spell1() {
        yield return WaitForFixedDuration(1f);
        health = 2500;
        maxhealth = 2500;
        Coroutine aux1 = StartCoroutine("Spell1_aux");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Spell1_aux() {
        for (float i = BOUND_Y; i > -BOUND_Y; i -= Random.Range(0.5f,1f)) {
            for (float j = -BOUND_X; j < BOUND_X; j += Random.Range(0.5f,2f)) {
                SpawnStraightBullet(petal_pool,new Vector3(j, i+Random.Range(-0.5f,0.5f), 0f),
                    -Vector2.up,
                    0.25f, 0.05f,
                    rotation:90f + Random.Range(-10f,10f)
                );
            }
            yield return WaitForFixedDuration(0.05f);
        }
        yield return WaitForFixedDuration(1f);
    }
}
