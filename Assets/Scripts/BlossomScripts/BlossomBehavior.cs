using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlossomBehavior : EnemyBehavior
{
    public static event EventHandler Thunderclap;
    [SerializeField] AudioClip thunderClapSFX;

    public Vector2 ambientWindDir;

    [Header("Bullet Config")]
    public GameObject wind_gust;
    public GameObject bul_petal;
    public GameObject bul_arrow;
    public GameObject bul_lightning_arrow;
    public GameObject bul_static_particle;
    public GameObject bul_ball_sm_black;
    public GameObject[] petal_pool;
    public GameObject[] arrow_pool;
    public GameObject[] lighting_arrow_pool;
    public GameObject[] static_particle_pool;
    public GameObject[] ball_blk_pool;

    protected override void InitEnemy()
    {
        petal_pool = gameManager.CreatePool(bul_petal, 1000);
        arrow_pool = gameManager.CreatePool(bul_arrow, 1000);
        lighting_arrow_pool = gameManager.CreatePool(bul_lightning_arrow, 50);
        static_particle_pool = gameManager.CreatePool(bul_static_particle, 1000);
        ball_blk_pool = gameManager.CreatePool(bul_ball_sm_black, 1000);
    }

    public IEnumerator Spell1()
    {
        yield return WaitForFixedDuration(1f);
        health = 2500;
        maxhealth = 2500;
        yield return MoveTo(new Vector2(0f, 2f));
        Coroutine aux1 = StartCoroutine("Spell1_aux1");
        Coroutine aux2 = StartCoroutine("Spell1_aux2");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Spell1_aux1()
    {
        yield return new WaitForSeconds(1f);
        ThunderstrikeTransition();
        
        float elapsed = 0f;
        while (true)
        {
            ambientWindDir = new Vector2(Mathf.Lerp(0, 1f, elapsed/20f), 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Spell1_aux2()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            Vector2 spawnPos = new Vector2(player.transform.position.x + Random.Range(-1f, 1f), BOUND_Y);
            GameObject lightningHead = SpawnActionBullet(lighting_arrow_pool, spawnPos, Vector2.down, 18f, 180f, 1.5f);
            lightningHead.GetComponent<ActionBulletBehavior>().AddInterval(
                (self) =>
                {
                    SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire3SFX, 0.35f);
                    for (int i = 0; i < 3; i ++)
                    {
                        Vector2 r = Random.insideUnitCircle;
                        r = new Vector2(r.x, Mathf.Abs(r.y));

                        SpawnStraightBullet(static_particle_pool, self.transform.position, -self.transform.up + (Vector3)r, Random.Range(0.25f, 2.5f));
                    }
                },
                0.08f
            );

            lightningHead.GetComponent<ActionBulletBehavior>().AddInterval( 
                (self) =>
                {
                    float turn = (24f + Random.Range(-8f, 8f)) * (self.transform.eulerAngles.z < 180f ? 1f : -1f);
                    ActionBulletBehavior abb = self.GetComponent<ActionBulletBehavior>();
                    abb.direction = RotateVector(abb.direction, turn);
                    self.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(abb.direction.y, abb.direction.x) * Mathf.Rad2Deg - 90f);
                    return;
                },
                0.18f
            );

            Transform toFollow = lightningHead.transform;
            for (int i = 0; i < 4; i ++)
            {
                toFollow = SpawnFollowerBullet(lighting_arrow_pool, toFollow, 2).transform;
            }
        }
    }


    public IEnumerator Interlude1()
    {
        yield return WaitForFixedDuration(1f);
        health = 1;
        maxhealth = 1;
        Coroutine aux1 = StartCoroutine("Inter1_aux");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Inter1_aux()
    {
        float petal_buffer_width = 6f;
        for (float i = BOUND_Y + petal_buffer_width; i > -BOUND_Y - petal_buffer_width; i -= Random.Range(0.5f, 2f))
        {
            for (float j = -BOUND_X - petal_buffer_width + Random.Range(0f, 0.5f); j < BOUND_X + petal_buffer_width; j += Random.Range(1f, 2f))
            {
                SpawnStraightBullet(petal_pool, new Vector3(j, i + Random.Range(-0.5f, 0.5f), 0f),
                    Vector2.down,
                    0.25f, 0.25f,
                    rotation: 90f + Random.Range(-10f, 10f)
                );
            }
            yield return WaitForFixedDuration(0.05f);
        }

        while (true)
        {
            StartCoroutine("Inter2_aux");
            yield return new WaitForSeconds(5.5f);

            GameObject gust = SpawnStraightBullet(wind_gust, new Vector2(0f, 12f), Vector3.down, 16f, -1f, false, 180f + Random.Range(-12f, 12f));
            gust.GetComponent<WindGustBehavior>().SetForce(3f);
            gust.transform.localScale = new Vector3(24f, 3f, 1f);
            yield return new WaitForSeconds(4f);
            Destroy(gust);
            yield return RandomMove();
        }
    }

    IEnumerator Inter2_aux()
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

                SpawnKeyedBullet(arrow_pool, transform.position, rKFs, sKFs, 0.35f);
            }

            SpawnCircleRing(transform.position, ball_blk_pool, 8, i*28f, 0.5f,
            (GameObject[] pool, Vector3 pos, Vector2 dir, float spd, float delay, float rot) =>
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
                    [0] = rot
                };
                SpawnKeyedBullet(pool, pos, rKFs, sKFs, delay);
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
