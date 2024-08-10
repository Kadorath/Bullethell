using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyBehavior : BHEntity
{
    [SerializeField] protected int health = 25000;
    protected int maxhealth = 25000;
    protected bool transition = false;
    protected string[] patterns;
    protected Coroutine cur_pattern;
    protected int pattern_ind = 0;

    private Image healthbar;
    public TextMeshProUGUI timer;

    public GameObject player;

    protected GameManagerBehavior gameManager;

    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();

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

        if (health == 0) {
            NextPattern();
        }
    }

    protected void NextPattern() {
        // Clear all enemy bullets
        GameObject[] all_bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bul in all_bullets) {
            bul.SetActive(false);
        }
        pattern_ind ++; 
        health = -1;
        StopAllCoroutines();
        cur_pattern = null;
    }

    // I have the x and y params for Atan2 swapped, look into this...
    protected float angleToPlayer(Vector3 a) {
        Vector2 dir = a - player.transform.position;
        float angle = Mathf.Atan2(dir.x, dir.y) + Mathf.PI/2;
        if (angle < 0) { angle += 2*Mathf.PI; }
        return angle*-1;
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
        for (float dist = total_dist; dist >= 0.001f;
            dist = Vector2.Distance(bul.transform.position,target_pos)) {
                bul.transform.Translate(((Vector3)target_pos-bul.transform.position)
                    *speed*Time.fixedDeltaTime, Space.World);
                    bul.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,Mathf.Lerp(start_rot,target_angle,1 - dist/total_dist)));
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

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.SetActive(false);
            if (health > 0) { health -= 1; }
        }
    }
}
