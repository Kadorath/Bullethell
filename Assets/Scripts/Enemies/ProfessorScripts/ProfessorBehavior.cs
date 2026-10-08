using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProfessorBehavior : EnemyBehavior
{
    [Header("Bullet Config")]
    public GameObject bul_cut;
    public GameObject bul_thrust;
    public GameObject[] cut_pool;
    public GameObject[] thrust_pool;

    protected override void InitEnemy()
    {
        cut_pool = gameManager.CreatePool(bul_cut, 1000);
        thrust_pool = gameManager.CreatePool(bul_thrust, 1000);
    }

    // Blossfechten
    public IEnumerator Spell1()
    {
        yield return WaitForFixedDuration(1f);
        health = 5000;
        maxhealth = 5000;
        yield return MoveTo(new Vector2(0f, 2f));
        Coroutine aux1 = StartCoroutine("Spell1_aux1");
        yield return PatternTimer(90f);
        NextPattern();
    }

    IEnumerator Spell1_aux1()
    {
        yield return new WaitForSeconds(1f);
        while(true)
        {
            // Vector2 vertOffset = Random.insideUnitCircle.normalized * 1.5f;
            // vertOffset = new Vector2(vertOffset.x, Mathf.Abs(vertOffset.y)) * 1.5f;
            // Vector2 moveTarget = (Vector2)player.transform.position + vertOffset;
            Vector2 moveTarget = player.transform.position + ((transform.position-player.transform.position).normalized * 2.5f);
            
            yield return MoveTo(moveTarget, 0.4f);
            yield return new WaitForSeconds(0.5f);

            Vector2 toPlayer = (player.transform.position - transform.position).normalized;
            StartCoroutine(Ort(toPlayer));

            int strike = Random.Range(0, 5);
            switch (strike)
            {
                case 0:
                    float dist = Math.Min(1.5f, Vector2.Distance(player.transform.position, transform.position)-0.2f);
                    StartCoroutine(Cut_Zornhau(toPlayer, distance:dist));
                    break;
                case 1:
                    StartCoroutine(Cut_Krumphau(toPlayer, Random.value < 0.5f));
                    break;
                case 2:
                    yield return StartCoroutine(Cut_Zwerchhau(toPlayer));
                    toPlayer = (player.transform.position - transform.position).normalized;
                    yield return new WaitForSeconds(0.25f);
                    yield return StartCoroutine(Cut_Zwerchhau(toPlayer, true, distance:2.2f));
                    toPlayer = (player.transform.position - transform.position).normalized;
                    yield return new WaitForSeconds(0.25f);
                    yield return StartCoroutine(Cut_Zwerchhau(toPlayer, distance:2.4f));
                    break;
                case 3:
                    StartCoroutine(Cut_Schielhau(toPlayer, Random.value < 0.5f));
                    break;
                case 4:
                    StartCoroutine(Cut_Scheitelhau(toPlayer));
                    break;
            }

            // yield return new WaitForSeconds(2f);
            // yield return MoveTo(new Vector2(0f, 2f));
            yield return new WaitForSeconds(Random.Range(0.75f, 2f));
        }
    }

    IEnumerator Cut_Zornhau(Vector2 direction, float length=6f, float distance=2f)
    {
        Vector2 cutDirection = new Vector2(-direction.y, direction.x);
        Vector2 cutCenter = (Vector2)transform.position + (direction * distance);
        Vector2 startPoint = cutCenter - (length/2 * cutDirection);
        Vector2 endPoint = cutCenter + (length/2 * cutDirection);
        Debug.DrawLine(startPoint, endPoint, Color.blue, 4f);
        Debug.DrawLine(transform.position, transform.position+(Vector3)(direction*distance), Color.white, 4f);

        foreach (Collider2D hit in Physics2D.OverlapAreaAll(startPoint-direction*0.3f, endPoint+direction*0.3f, LayerMask.GetMask("PlayerBullets")))
        {
            hit.gameObject.SetActive(false);
        }

        int bul_count = 24;
        float half_bc = bul_count/2f;
        for (int i = 0; i <= bul_count; i ++)
        {
            float t = ((float)i)/bul_count;
            Vector2 pos = new Vector2(Mathf.Lerp(startPoint.x, endPoint.x, t), Mathf.Lerp(startPoint.y, endPoint.y, t));
            pos += direction * 0.3f * (-i*(i-bul_count) / (half_bc * half_bc));
            SpawnKeyedBullet(cut_pool, pos, 
                new Dictionary<int, float>{ [0] = Random.Range(0, 360f) },
                new Dictionary<int, float>{ [0] = 0, [20] = 0, [60] = Random.Range(1f, 4f) }
            );
            SpawnKeyedBullet(cut_pool, pos + direction*0.2f, 
                new Dictionary<int, float>{ [0] = Random.Range(0, 360f) },
                new Dictionary<int, float>{ [0] = 0, [20] = 0, [60] = Random.Range(1f, 4f) }
            );
            SpawnKeyedBullet(cut_pool, pos + direction*0.4f, 
                new Dictionary<int, float>{ [0] = Random.Range(0, 360f) },
                new Dictionary<int, float>{ [0] = 0, [20] = 0, [60] = Random.Range(1f, 4f) }
            );
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator Cut_Krumphau(Vector2 direction, bool flip=false, float distance=2f)
    {
        Vector2 cutDirection = new Vector2(-direction.y, direction.x);
        Vector2 cutCenter = (Vector2)transform.position + direction*0.25f;
        float flipMod = flip ? -1f : 1f;
        int bul_count = 24;
        for (int i = 0; i <= bul_count; i ++)
        {
            float t = ((float)i)/bul_count;
            float c1 = 1.70158f;
            float c3 = c1+1;
            Vector2 pos = cutCenter;
            pos += Vector2.Lerp(flipMod*cutDirection*2f, Vector2.zero, t);
            pos += direction * distance*(1 + c3*Mathf.Pow(t-1,3) + c1*Mathf.Pow(t-1, 2f));
            SpawnKeyedBullet(cut_pool, pos, 
                new Dictionary<int, float>{ [0] = Random.Range(0, 360f) },
                new Dictionary<int, float>{ [0] = 0, [20] = 0, [60] = Random.Range(1f, 4f) }
            );
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator Cut_Zwerchhau(Vector2 direction, bool flip=false, float length=3f, float distance=2f)
    {
        Vector2 cutDirection = new Vector2(-direction.y, direction.x);
        Vector2 cutCenter = (Vector2)transform.position + (direction * distance);
        Vector2 startPoint = cutCenter - (length/2 * cutDirection);
        Vector2 endPoint = cutCenter + (length/2 * cutDirection);
        Debug.DrawLine(startPoint, endPoint, Color.blue, 4f);
        Debug.DrawLine(transform.position, transform.position+(Vector3)(direction*distance), Color.white, 4f);

        foreach (Collider2D hit in Physics2D.OverlapAreaAll(startPoint-direction*0.1f, endPoint+direction*0.1f, LayerMask.GetMask("PlayerBullets")))
        {
            hit.gameObject.SetActive(false);
        }

        int bul_count = 24;
        float half_bc = bul_count/2f;
        for (int i = 0; i <= bul_count; i ++)
        {
            float t = !flip ? (((float)i)/bul_count) : (1f - ((float)i/bul_count));
            Vector2 pos = new Vector2(Mathf.Lerp(startPoint.x, endPoint.x, t), Mathf.Lerp(startPoint.y, endPoint.y, t));
            pos += direction * 0.3f * (-i*(i-bul_count) / (half_bc * half_bc));
            SpawnKeyedBullet(cut_pool, pos, 
                new Dictionary<int, float>{ [0] = Random.Range(0, 360f) },
                new Dictionary<int, float>{ [0] = 0, [20] = 0, [60] = Random.Range(1f, 4f) }
            );
            yield return new WaitForSeconds(0.002f);
        }
    }

    IEnumerator Cut_Schielhau(Vector2 direction, bool flip=false, float length=3f, float distance=2f)
    {
        int flipMod = flip ? -1 : 1;
        Vector2 cutDirection = new Vector2(-direction.y, direction.x);
        Vector2 cutCenter = (Vector2)transform.position + (direction * distance);
        Vector2 startPoint = cutCenter - flipMod*(length*0.65f * cutDirection) - 2f*direction;
        Vector2 endPoint = cutCenter + flipMod*(length*0.35f * cutDirection) + 1f*direction;
        Debug.DrawLine(startPoint, endPoint, Color.blue, 4f);
        Debug.DrawLine(transform.position, transform.position+(Vector3)(direction*distance), Color.white, 4f);

        foreach (Collider2D hit in Physics2D.OverlapAreaAll(startPoint-direction*0.3f, endPoint+direction*0.3f, LayerMask.GetMask("PlayerBullets")))
        {
            hit.gameObject.SetActive(false);
        }

        int bul_count = 24;
        float half_bc = bul_count/2f;
        for (int i = 0; i <= bul_count; i ++)
        {
            float t = ((float)i)/bul_count;
            Vector2 pos = new Vector2(Mathf.Lerp(startPoint.x, endPoint.x, t), Mathf.Lerp(startPoint.y, endPoint.y, t));
            pos += direction * 0.3f * (-i*(i-bul_count) / (half_bc * half_bc));
            SpawnKeyedBullet(cut_pool, pos, 
                new Dictionary<int, float>{ [0] = Random.Range(0, 360f) },
                new Dictionary<int, float>{ [0] = 0, [20] = 0, [60] = Random.Range(1f, 4f) }
            );
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator Cut_Scheitelhau(Vector2 direction, float length=3f)
    {
        Vector2 startPoint = (Vector2)transform.position - direction*0.5f;
        Vector2 endPoint = (Vector2)transform.position + direction * length;
        int bul_count = 32;
        for (int i = 0; i <= bul_count; i ++)
        {
            float t = InOutBack(((float)i)/bul_count);
            Vector2 pos = Vector2.LerpUnclamped(startPoint, endPoint, t);
            SpawnKeyedBullet(cut_pool, pos, 
                new Dictionary<int, float>{ [0] = Random.Range(0, 360f) },
                new Dictionary<int, float>{ [0] = 0, [20] = 0, [60] = Random.Range(1f, 4f) }
            );
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator Ort(Vector2 direction)
    {
        Vector2 line = new Vector2(-direction.y, direction.x);
        Vector2 cutCenter = (Vector2)transform.position;
        Vector2 startPoint = cutCenter - (0.5f * line);
        Vector2 endPoint = cutCenter + (0.5f * line);
        Debug.DrawLine(transform.position, (Vector2)transform.position+direction, Color.blue, 4f);

        int bul_count = 9;
        for (int i = 0; i <= bul_count; i ++)
        {
            float t = ((float)i)/bul_count;
            Vector2 pos = new Vector2(Mathf.Lerp(startPoint.x, endPoint.x, t), Mathf.Lerp(startPoint.y, endPoint.y, t));
            float speed = 1f + -Mathf.Abs(i-(bul_count/2f)) + (bul_count/2f);
            SpawnStraightBullet(thrust_pool, pos, direction, speed, rotation:Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
        }

        yield return null;
    }
}
