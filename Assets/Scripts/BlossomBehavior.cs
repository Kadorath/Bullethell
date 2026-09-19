using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlossomBehavior : EnemyBehavior
{
    public static event EventHandler Thunderclap;
    [SerializeField] AudioClip thunderClapSFX;
    private static Vector2 _ambientWindDir;
    public static Vector2 AmbientWindDir { get => _ambientWindDir; }

    [Header("Bullet Config")]
    public GameObject bul_default;
    public Sprite spr_darkblue;
    public Sprite spr_black;
    public Sprite spr_ball_black;
    public GameObject wind_gust;
    public GameObject bul_petal;
    private GameObject[] darkblue_pool;
    private GameObject[] black_pool;
    private GameObject[] ball_black_pool;
    private GameObject[] petal_pool;

    protected override void InitEnemy()
    {
        GameObject blue_template = Instantiate(bul_default);
        blue_template.GetComponentInChildren<SpriteRenderer>().sprite = spr_darkblue;
        blue_template.name = "Blue Arrow";
        GameObject black_template = Instantiate(bul_default);
        black_template.GetComponentInChildren<SpriteRenderer>().sprite = spr_black;
        black_template.name = "Black Arrow";
        GameObject ball_black_template = Instantiate(bul_default);
        ball_black_template.GetComponentInChildren<SpriteRenderer>().sprite = spr_ball_black;
        ball_black_template.name = "Black Small Ball";
        
        darkblue_pool = gameManager.CreatePool(blue_template, 2000);
        black_pool = gameManager.CreatePool(black_template, 300);
        ball_black_pool = gameManager.CreatePool(ball_black_template, 1000);
        Destroy(black_template);
        Destroy(blue_template);
        Destroy(ball_black_template);

        petal_pool = gameManager.CreatePool(bul_petal, 1000);
    }
    public IEnumerator Intro()
    {
        yield return WaitForFixedDuration(1f);
        health = -1;
        maxhealth = -1;
        float petal_buffer_width = 4f;
        for (float i = BOUND_Y + petal_buffer_width; i > -BOUND_Y - petal_buffer_width - 3f; i -= Random.Range(0.5f, 2f))
        {
            for (float j = -BOUND_X - petal_buffer_width + Random.Range(0f, 0.5f); j < BOUND_X + petal_buffer_width; j += Random.Range(1.5f, 2.5f))
            {
                if (Random.value < 0.25f) continue;
                SpawnStraightBullet(petal_pool, new Vector3(j, i + Random.Range(-0.5f, 0.5f), 0f),
                    direction:Vector2.down,
                    speed:0.25f, delay:0f,
                    rotation: 90f + Random.Range(-10f, 10f)
                );
            }
        }
        yield return WaitForFixedDuration(0.6f);
        GameObject gust = SpawnStraightBullet(wind_gust, new Vector2(0f, 11f), Vector3.down, 6f);
        gust.GetComponent<WindGustBehavior>().SetForce(0f);
        gust.transform.localScale = new Vector3(24f, 3f, 1f);
        yield return WaitForFixedDuration(3f);
        NextPattern();
    }

    public IEnumerator Interlude1()
    {
        yield return WaitForFixedDuration(1f);
        health = 5000;
        maxhealth = 5000;
        yield return MoveTo(new Vector2(0f, 2f));
        Coroutine aux1 = StartCoroutine("Inter1_aux1");
        yield return PatternTimer(90f);
        NextPattern();
    }

    public IEnumerator Interlude2()
    {
        yield return WaitForFixedDuration(1f);
        health = 5000;
        maxhealth = 5000;
        yield return MoveTo(new Vector2(0f, 2f));
        Coroutine aux1 = StartCoroutine("Inter2_aux1");
        yield return PatternTimer(90f);
        NextPattern();
    }

    public IEnumerator Interlude3()
    {
        yield return WaitForFixedDuration(1f);
        health = 5000;
        maxhealth = 5000;
        yield return MoveTo(new Vector2(0f, 2f));
        Coroutine aux1 = StartCoroutine("Inter3_aux1");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Inter1_aux1()
    {
        Inter1_aux2();
        int bul_num = 18;
        int ct = 0;
        while (true)
        {
            for (int wave = 0; wave < 12; wave ++)
            {
                float rot = RADTODEG*(0.05f*wave);
                SpawnCircleRing(
                    darkblue_pool, transform.position, bul_num:bul_num,
                    rotation:rot, speed:4f, delay:0.25f,
                    speedChangeDuration:3f, startSpeedMod:1.5f, finalSpeedMod:-1.5f
                );
                yield return WaitForFixedDuration(0.075f);
            }


            for (int wave = 0; wave < 12; wave ++)
            {
                float rot = RADTODEG*(2*Mathf.PI - (0.05f*wave));
                SpawnCircleRing(
                    darkblue_pool, transform.position, bul_num:bul_num,
                    rotation:rot, speed:4f, delay:0.25f,
                    speedChangeDuration:3f, startSpeedMod:1.5f, finalSpeedMod:-1f
                );
                yield return WaitForFixedDuration(0.075f);
            }

            Inter1_aux2(8+(ct/3));
            yield return WaitForFixedDuration(1.5f);
            yield return RandomMove();
            yield return WaitForFixedDuration(0.5f);
            ct += 1;
        }
    }
    private void Inter1_aux2(int bul_num = 8)
    {
        float r = Random.Range(0, 360);
        SpawnCircleRing(
            black_pool, transform.position, bul_num:bul_num, 
            rotation:r, speed:3f, delay:0.25f, 
            speedChangeDuration:9f, startSpeedMod:1f, finalSpeedMod:-0.25f
        );
    }

    IEnumerator Inter2_aux1()
    {
        float spinFactor = 0.8f;
        int bul_num = 28;
        while (true)
        {
            for (int i = 0; i < 2; i ++)
            {
                SpawnCircleRing(
                    darkblue_pool, transform.position, bul_num:bul_num,
                    rotation:0f, speed:2f*spinFactor, turnSpeed:16f*spinFactor, delay:0.25f,
                    speedChangeDuration:0.5f-(i*0.1f), startSpeedMod:3f, finalSpeedMod:1f
                );
                SpawnCircleRing(
                    darkblue_pool, transform.position, bul_num:bul_num,
                    rotation:0f, speed:2f*spinFactor, turnSpeed:-16f*spinFactor, delay:0.25f,
                    speedChangeDuration:0.5f-(i*0.1f), startSpeedMod:3f, finalSpeedMod:1f
                );
                yield return WaitForFixedDuration(0.2f);
            }
            yield return WaitForFixedDuration(2f);
        }
    }

    IEnumerator Inter3_aux1()
    {
        while (true)
        {
            StartCoroutine("Inter3_aux2");
            yield return new WaitForSeconds(5.5f);

            GameObject gust = SpawnStraightBullet(wind_gust, new Vector2(0f, 12f), Vector3.down, 16f, -1f, 180f + Random.Range(-12f, 12f));
            gust.GetComponent<WindGustBehavior>().SetForce(3f);
            gust.transform.localScale = new Vector3(24f, 3f, 1f);
            yield return new WaitForSeconds(4f);
            Destroy(gust);
            yield return RandomMove();
        }
    }
    IEnumerator Inter3_aux2()
    {
        int arm_num = 8;
        int bul_num = 24;
        for (int i = 0; i < bul_num; i ++)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire3SFX);
            for (int arm = 0; arm < arm_num; arm ++)
            {
                float baseRot = RADTODEG * arm*(2f*Mathf.PI/(arm_num-1)) + i*12f;
                Dictionary<int, float> rKFs = new Dictionary<int, float>{
                    [0] = baseRot, 
                    [50] = baseRot+180,
                    [150] = baseRot+620,
                    [200] = baseRot+620+Random.Range(-24f, 24f)};

                Dictionary<int, float> sKFs = new Dictionary<int, float>
                {
                    [0] = 5 - Mathf.Lerp(0, 3, i/32),
                    [100] = 5 - Mathf.Lerp(0, 3, i/32),
                    [200] = 0,
                    [250] = 0,
                    [320] = 5
                };

                SpawnKeyedBullet(darkblue_pool, transform.position, rKFs, sKFs, 0.35f);
            }

            SpawnCircleRing(8, i*28f,
                (Vector2 direction, float rotation) =>
                {
                    Dictionary<int, float> sKFs = new Dictionary<int, float>
                    {
                        [0] = 6,
                        [10] = 6,
                        [40] = 0.75f,
                        [400] = 0.75f,
                        [650] = 4f
                    };
                    Dictionary<int, float> rKFs = new Dictionary<int, float>
                    {
                        [0] = rotation
                    };
                    SpawnKeyedBullet(ball_black_pool, transform.position, rKFs, sKFs, 0.15f);
                });
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ThunderstrikeTransition()
    {
        SoundManager.Instance.PlayOneShot(thunderClapSFX, 0.65f);
        Thunderclap?.Invoke(this, EventArgs.Empty);
    }
}