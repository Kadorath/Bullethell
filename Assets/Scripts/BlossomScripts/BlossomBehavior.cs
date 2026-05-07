using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlossomBehavior : EnemyBehavior
{
    public static event EventHandler Thunderclap;

    public GameObject bul_petal;
    public GameObject wind_gust;
    public GameObject[] petal_pool;

    protected override void InitEnemy()
    {
        petal_pool = gameManager.CreatePool(bul_petal, 1000);
    }

    IEnumerator Spell2()
    {
        yield return WaitForFixedDuration(1f);
        health = 2500;
        maxhealth = 2500;
        Coroutine aux1 = StartCoroutine("Spell2_aux");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Spell2_aux()
    {
        SpawnStraightBullet(petal_pool, new Vector3(0f, 0f, 0f),
                    -Vector2.up * 0.25f,
                    0.25f, 0.05f,
                    rotation: 90f + Random.Range(-10f, 10f)
                );

        yield return new WaitForSeconds(1f);
        ThunderstrikeTransition();
    }


    IEnumerator Spell1()
    {
        yield return WaitForFixedDuration(1f);
        health = 2500;
        maxhealth = 2500;
        Coroutine aux1 = StartCoroutine("Spell1_aux");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Spell1_aux()
    {
        float petal_buffer_width = 6f;
        for (float i = BOUND_Y + petal_buffer_width; i > -BOUND_Y - petal_buffer_width; i -= Random.Range(0.5f, 1f))
        {
            for (float j = -BOUND_X - petal_buffer_width; j < BOUND_X + petal_buffer_width; j += Random.Range(0.5f, 2f))
            {
                SpawnStraightBullet(petal_pool, new Vector3(j, i + Random.Range(-0.5f, 0.5f), 0f),
                    Vector2.down,
                    0.25f, 0.05f,
                    rotation: 90f + Random.Range(-10f, 10f)
                );
            }
            yield return WaitForFixedDuration(0.05f);
        }
        yield return WaitForFixedDuration(2f);

        while (true)
        {
            GameObject gust = SpawnStraightBullet(wind_gust, new Vector2(0f, 12f), Vector3.down, 16f, -1f, false, 180f);
            gust.GetComponent<WindGustBehavior>().SetForce(3f);
            gust.transform.localScale = new Vector3(12f, 3f, 1f);
            yield return WaitForFixedDuration(6f);
            Destroy(gust);

            ThunderstrikeTransition();

            gust = SpawnStraightBullet(wind_gust, new Vector2(-12f, 0f), Vector3.right, 36f, -1f, false, -90f);
            gust.GetComponent<WindGustBehavior>().SetForce(1f);
            gust.transform.localScale = new Vector3(24f, 3f, 1f);
            yield return WaitForFixedDuration(6f);

            ThunderstrikeTransition();
        }
    }

    private void ThunderstrikeTransition()
    {
        Thunderclap?.Invoke(this, EventArgs.Empty);
    }
}
