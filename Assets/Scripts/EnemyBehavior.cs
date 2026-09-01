using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using Random = UnityEngine.Random;

public class EnemyBehavior : BHEntity
{
    [SerializeField] protected int health = 25000;
    protected int maxhealth = 25000;
    protected bool transition = false;
    [SerializeField] protected string[] patterns;
    protected Coroutine cur_pattern;
    protected int pattern_ind = 0;

    private Collider2D[] collided_bul;

    private Image healthbar;
    public TextMeshProUGUI timer;

    public GameObject player;
    
    protected GameManager gameManager;

    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameManager.Instance;

        healthbar = GameObject.Find("EnemyHealthbar").GetComponent<Image>();
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        InitEnemy();
    }

    protected virtual void InitEnemy() { }

    void FixedUpdate()
    {
        if (health == -1) {
            healthbar.fillAmount = 0f;
        }
        else {
            healthbar.fillAmount = health / (float)maxhealth;
        }
        if (cur_pattern == null) {
            cur_pattern = StartCoroutine(patterns[pattern_ind]);
        }

        // Player bullet collision detection
        collided_bul = Physics2D.OverlapBoxAll(transform.position, new Vector2(2f, 2f), 0f, LayerMask.GetMask("PlayerBullets"));
        foreach (Collider2D bul in collided_bul) {
            bul.gameObject.SetActive(false);
            gameManager.AddScore(10);
            if (health > 0) { health -= 1; }
        }
        if (health == 0) {
            NextPattern();
        }
    }

    protected void NextPattern() {
        // Clear all enemy bullets
        GameObject[] all_bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bul in all_bullets) {
            // TODO: make this accomodate snapshot frame
            foreach(BulletBehavior bb in bul.GetComponents<BulletBehavior>()) 
                bb.DestroySelf();
        }

        pattern_ind ++; 
        health = -1;
        StopAllCoroutines();
        cur_pattern = null;
    }

    // I have the x and y params for Atan2 swapped, look into this...
    protected float angleToPlayer(Vector3 a) {
        Vector2 dir = player.transform.position - a;
        float angle = Mathf.Atan2(dir.y, dir.x);
        if (angle < 0) { angle += 2*Mathf.PI; }
        return angle;
    }

    protected IEnumerator PatternTimer(float duration) {
        for (float d = duration; d > 0f; d -= Time.fixedDeltaTime) {
            timer.text = d.ToString("F2");
            timer.text = timer.text.Insert(timer.text.IndexOf("."),"<size=50%>");
            
            yield return new WaitForFixedUpdate();
        }
        timer.text = "0.<size=50%>00";
    }

    protected IEnumerator RandomMove() {
        float angle = Random.Range(0,2*Mathf.PI);
        Vector2 dest = new Vector2(transform.position.x+1.5f*Mathf.Cos(angle),
            transform.position.y+1.5f*Mathf.Sin(angle));
        while(dest.y < 2f || dest.y > 4.5f|| Mathf.Abs(dest.x) > 3f) {
            angle = Random.Range(0,2*Mathf.PI);
            dest = new Vector2(transform.position.x+1.5f*Mathf.Cos(angle),
                transform.position.y+1.5f*Mathf.Sin(angle));
        }

        while (Vector2.Distance(transform.position, dest) >= 0.01f) {
            transform.Translate(((Vector3)dest - transform.position)*3f*Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    protected IEnumerator MoveTo(Vector2 dest) {
        while (Vector2.Distance(transform.position, dest) >= 0.01f) {
            transform.Translate(((Vector3)dest - transform.position)*3f*Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    protected IEnumerator MoveTo(Vector2 dest, float duration)
    {
        Vector2 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed <= duration)
        {
            elapsed += Time.deltaTime;
            float t = InOutCubic(elapsed/duration);
            transform.position = Vector2.Lerp(startPos, dest, t);
            yield return null;
        }

        transform.position = dest;
    }

    protected IEnumerator MoveBullet(GameObject bul, float speed, Vector2 target_pos, float target_angle, Vector2 target_scale) {
        float total_dist = Vector2.Distance(bul.transform.position,target_pos);
        float start_rot = bul.transform.eulerAngles.z;
        float start_scale_x = bul.transform.localScale.x;
        float start_scale_y = bul.transform.localScale.y;
        for (float dist = total_dist; dist >= 0.001f;
            dist = Vector2.Distance(bul.transform.position,target_pos)) {
                bul.transform.Translate(((Vector3)target_pos-bul.transform.position)
                    *speed*Time.fixedDeltaTime, Space.World);
            bul.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,Mathf.Lerp(start_rot,target_angle,1 - dist/total_dist)));
            bul.transform.localScale = new Vector3(Mathf.Lerp(start_scale_x, target_scale.x, 1 - dist/total_dist),
                                                Mathf.Lerp(start_scale_y, target_scale.y, 1 - dist/total_dist),
                                                1f);
            yield return new WaitForFixedUpdate();
        }        
    }

    protected IEnumerator MoveBullet(GameObject bul, float speed, Vector2 target_pos, float target_angle) {
        float total_dist = Vector2.Distance(bul.transform.position,target_pos);
        float start_rot = bul.transform.eulerAngles.z;
        float angleToTurn = Mathf.DeltaAngle(start_rot, target_angle);
        for (float dist = total_dist; dist >= 0.001f;
            dist = Vector2.Distance(bul.transform.position,target_pos)) {
                bul.transform.Translate(((Vector3)target_pos-bul.transform.position)
                    *speed*Time.fixedDeltaTime, Space.World);
                    bul.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,start_rot + Mathf.Lerp(0f, angleToTurn, 1 - dist/total_dist)));
            yield return new WaitForFixedUpdate();
        }
    }

    protected IEnumerator MoveBullet(GameObject bul, float speed, Vector2 target_pos) {
        float total_dist = Vector2.Distance(bul.transform.position,target_pos);
        for (float dist = total_dist; dist >= 0.001f;
            dist = Vector2.Distance(bul.transform.position,target_pos)) {
                bul.transform.Translate(((Vector3)target_pos-bul.transform.position)
                    *speed*Time.fixedDeltaTime, Space.World);
            yield return new WaitForFixedUpdate();
        }     
    }

    protected Vector2 RotateVector(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    protected void SpawnCircleRing(Vector3 pos, GameObject[] bul_pool, int bul_num, float rot, float s)
    {
        for (int i = 0; i < bul_num; i ++)
        {
            float angle = i * 360f / bul_num + rot;
            Vector2 direction = RotateVector(Vector2.up, angle);
            SpawnStraightBullet(bul_pool, pos, direction, s, 0.15f, rotation:angle);
        }
    }

    protected void SpawnCircleRing(Vector3 pos, GameObject[] bul_pool, int bul_num, float rot, float s, Action<GameObject[], Vector3, Vector2, float, float, float> spawnMethod)
    {
        for (int i = 0; i < bul_num; i ++)
        {
            float angle = i * 360f / bul_num + rot;
            Vector2 direction = RotateVector(Vector2.up, angle);
            spawnMethod(bul_pool, pos, direction, s, 0.15f, angle);
        }
    }

    protected void SpawnSquareRing(Vector3 pos, GameObject[] bul_pool, int bul_num, float rot, float s)
    {
        for (int i = 0; i < bul_num; i ++)
        {
            float squareOffset = Mathf.Lerp(1f, -1f, i/(bul_num-1f));
            for (int side = 0; side < 4; side ++) {
                Vector2 direction = new Vector2(side%2==0 ? squareOffset : (side < 2 ? 1f : -1f), side%2!=0 ? squareOffset : (side < 2 ? 1f : -1f)).normalized;
                direction = RotateVector(direction, rot);
                float baseSpeed = s;
                float speed = baseSpeed * Mathf.Sqrt(1f+Mathf.Pow(Mathf.Lerp(0f, 1f, Mathf.Abs((i/((bul_num-1)/2f)) - 1f)), 2));
                SpawnStraightBullet(bul_pool, pos, direction, speed, 0.15f, rotation:Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
            }
        }
    }

    /*void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.SetActive(false);
            if (health > 0) { health -= 1; }
        }
    }*/
}
