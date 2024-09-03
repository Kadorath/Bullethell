using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlossomBehavior : EnemyBehavior
{
    public GameObject bul_petal;
    public GameObject[] petal_pool;

    protected override void InitEnemy() {
        petal_pool = gameManager.CreatePool(bul_petal, 500);
     
        patterns = new string[] {"Spell1"};
    }

    IEnumerator Spell1() {
        yield return WaitForFixedDuration(2f);
        health = 2500;
        maxhealth = 2500;
        Coroutine aux1 = StartCoroutine("Spell1_aux");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Spell1_aux() {
        yield return WaitForFixedDuration(1f);
    }
}
