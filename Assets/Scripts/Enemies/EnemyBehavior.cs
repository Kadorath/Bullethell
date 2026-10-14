using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using Random = UnityEngine.Random;

public class EnemyBehavior : BHEntity
{
    [Serializable]
    private class PooledBullet
    {
        public string Name;
        public GameObject Bullet;
        public Sprite Sprite;
        public int Amount;    
    }

    public class Pattern
    {
        public string Name;
        public int Health;
        public int Time;
        public bool IsSpell;
        public IEnumerator[] Routines;

        public Pattern(string name, int hp, int time, bool isSpell, params IEnumerator[] routines)
        {
            Name = name;
            Health = hp;
            Time = time;
            Routines = routines;
            IsSpell = isSpell;
        }
    }

    // Health
    [SerializeField] protected int _health = -1;
    protected int maxhealth = -1;
    protected bool transition = false;

    public int Health { get => _health; set => _health = value;}
    
    // Patterns
    protected Pattern[] _patternList;
    [SerializeField] protected string[] patterns;
    private Coroutine _curPattern;
    private int _patternInd = 0;

    // Bullets
    [SerializeField] List<PooledBullet> _bullets;
    protected Dictionary<string, GameObject[]> _bulletPools;

    private Collider2D[] _collidedBul;

    private Image healthbar;
    protected Animator gameUIAnim;
    protected RectTransform portrait;
    protected TextMeshProUGUI spellNameText;
    protected TextMeshProUGUI timer;
    protected GameObject player;
    protected GameManager gameManager;

    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameManager.Instance;

        healthbar = GameObject.Find("EnemyHealthbar").GetComponent<Image>();
        gameUIAnim = GameObject.Find("GameViewUI").GetComponent<Animator>();
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        portrait = GameObject.Find("Portrait").GetComponent<RectTransform>();
        spellNameText = GameObject.Find("SpellName").GetComponentInChildren<TextMeshProUGUI>();

        _bulletPools = new Dictionary<string, GameObject[]>();
        foreach (PooledBullet bulToPool in _bullets)
        {
            GameObject template = Instantiate(bulToPool.Bullet);
            template.name = string.IsNullOrEmpty(bulToPool.Name) ? template.name : bulToPool.Name;
            if (bulToPool.Sprite != null)
                template.GetComponentInChildren<SpriteRenderer>().sprite = bulToPool.Sprite;
            GameObject[] newPool = gameManager.CreatePool(template, bulToPool.Amount);
            _bulletPools[template.name] = newPool;
            Destroy(template);
        }

        InitEnemy();
    }

    protected virtual void InitEnemy() { }

    void FixedUpdate()
    {
        if (_health == -1) {
            healthbar.fillAmount = 0f;
        }
        else {
            healthbar.fillAmount = _health / (float)maxhealth;
        }
        if (_curPattern == null && _patternInd < patterns.Length) {
            Pattern patternToRun = FindPattern(patterns[_patternInd]);
            if (patternToRun != null)
                _curPattern = StartCoroutine(RunPattern(patternToRun));
        }

        // Player bullet collision detection
        _collidedBul = Physics2D.OverlapBoxAll(transform.position, new Vector2(2f, 2f), 0f, LayerMask.GetMask("PlayerBullets"));
        foreach (Collider2D bul in _collidedBul) {
            bul.gameObject.SetActive(false);
            gameManager.AddScore(10);
            if (_health > 0) { _health -= 1; }
        }
        if (_health == 0) {
            NextPattern();
        }
    }

    private Pattern FindPattern(string patternName)
    {
        if (_patternList == null) return null;

        foreach (Pattern p in _patternList)
            if (p.Name == patternName) return p;
        return null;
    }

    private IEnumerator RunPattern(Pattern pattern)
    {
        portrait.transform.position = new Vector3(500f, 300f, 0f);
        spellNameText.transform.parent.position = new Vector3(-150f, -600f, 0f);
        gameUIAnim.SetBool("spell", pattern.IsSpell);
        if (pattern.IsSpell)
            yield return IntroSpell(pattern);
        else
        {
            yield return WaitForFixedDuration(2f);
        }

        _health = pattern.Health;
        maxhealth = pattern.Health;
        foreach (IEnumerator routine in pattern.Routines)
            StartCoroutine(routine);

        if (pattern.Time == -1) yield break;
        yield return PatternTimer(pattern.Time);
        NextPattern();
    }
    private IEnumerator IntroSpell(Pattern pattern)
    {
        // Trigger UI for spells
        spellNameText.text = pattern.Name;
        gameUIAnim.SetTrigger("patternstart");
        yield return WaitForFixedDuration(2f);
    }
    protected void NextPattern() 
    {
        // Clear all enemy bullets
        GameObject[] all_bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bul in all_bullets) {
            // TODO: make this accomodate snapshot frame
            foreach(BulletBehavior bb in bul.GetComponents<BulletBehavior>()) 
                bb.DestroySelf();
        }

        _patternInd ++; 
        _health = -1;
        StopAllCoroutines();
        _curPattern = null;
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

    protected IEnumerator MoveBullet(GameObject bul, float duration, Vector3? targetPos=null, float? targetAngle=null, Vector2? targetScale=null, Func<float,float> easing=null)
    {
        if (easing == null) easing = InOutCubic;
        Vector3 startPos = bul.transform.position;
        float startAngle = bul.transform.eulerAngles.z;
        Vector2 startScale = bul.transform.localScale;

        float elapsed = 0f;
        while (elapsed <= duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = easing(elapsed/duration);
            if (targetPos.HasValue)
                bul.transform.position = Vector3.Lerp(startPos, targetPos.Value, t);
            if (targetAngle.HasValue)
                bul.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.LerpAngle(startAngle, targetAngle.Value, t)));
            if (targetScale.HasValue)
                bul.transform.localScale = Vector2.Lerp(startScale, targetScale.Value, t);
            yield return new WaitForFixedUpdate();
        }
    }

    // protected IEnumerator MoveBullet(GameObject bul, float speed, Vector2 target_pos, float target_angle, Vector2 target_scale) {
    //     float total_dist = Vector2.Distance(bul.transform.position,target_pos);
    //     float start_rot = bul.transform.eulerAngles.z;
    //     float start_scale_x = bul.transform.localScale.x;
    //     float start_scale_y = bul.transform.localScale.y;
    //     for (float dist = total_dist; dist >= 0.01f; // TODO: This needs to not just be based on distance.
    //         dist = Vector2.Distance(bul.transform.position,target_pos)) {
    //             bul.transform.Translate(((Vector3)target_pos-bul.transform.position)
    //                 *speed*Time.fixedDeltaTime, Space.World);
    //         bul.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,Mathf.Lerp(start_rot,target_angle,1 - dist/total_dist)));
    //         bul.transform.localScale = new Vector3(Mathf.Lerp(start_scale_x, target_scale.x, 1 - dist/total_dist),
    //                                             Mathf.Lerp(start_scale_y, target_scale.y, 1 - dist/total_dist),
    //                                             1f);
    //         yield return new WaitForFixedUpdate();
    //     }

    //     bul.transform.position = target_pos;
    //     bul.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,target_angle));
    //     bul.transform.localScale = new Vector3(target_scale.x, target_scale.y, 1f);
    // }

    // protected IEnumerator MoveBullet(GameObject bul, float speed, Vector2 target_pos, float target_angle) {
    //     float total_dist = Vector2.Distance(bul.transform.position,target_pos);
    //     float start_rot = bul.transform.eulerAngles.z;
    //     float angleToTurn = Mathf.DeltaAngle(start_rot, target_angle);
    //     for (float dist = total_dist; dist >= 0.1f;
    //         dist = Vector2.Distance(bul.transform.position,target_pos)) {
    //             bul.transform.Translate(((Vector3)target_pos-bul.transform.position)
    //                 *speed*Time.fixedDeltaTime, Space.World);
    //                 bul.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,start_rot + Mathf.Lerp(0f, angleToTurn, 1 - dist/total_dist)));
    //         yield return new WaitForFixedUpdate();
    //     }

    //     bul.transform.position = target_pos;
    //     bul.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,target_angle));
    // }

    // protected IEnumerator MoveBullet(GameObject bul, float speed, Vector2 target_pos) {
    //     float total_dist = Vector2.Distance(bul.transform.position,target_pos);
    //     for (float dist = total_dist; dist >= 0.001f;
    //         dist = Vector2.Distance(bul.transform.position,target_pos)) {
    //             bul.transform.Translate(((Vector3)target_pos-bul.transform.position)
    //                 *speed*Time.fixedDeltaTime, Space.World);
    //         yield return new WaitForFixedUpdate();
    //     }     
    // }

    protected Vector2 RotateVector(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    /// <summary>
    /// Spawns an even ring of bullets. The spawned bullets will be StraightBullets. For more specific spawning control, see method overloads.
    /// </summary>
    /// <param name="bul_pool">The object pool from which to draw bullets for the ring.</param>
    /// <param name="origin">The starting position bullets spawn at.</param>
    /// <param name="bul_num">The number of bullets to create for the ring. Bullets will be evenly distributed around the circle.</param>
    /// <param name="rotation">The starting rotation in degrees to begin constructing the ring of bullets.</param>
    /// <param name="speed">Speed of the spawned bullets</param>
    /// <param name="turnSpeed">Turn speed of the spawned bullets</param>
    /// <param name="delay">Spawn delay of the spawned bullets.</param>
    /// <param name="speedChangeDuration">Speed change duration of the spawned bullets.</param>
    /// <param name="startSpeedMod">Starting speed mod of the spawned bullets.</param>
    /// <param name="finalSpeedMod">Final speed mod of the spawned bullets.</param>
    protected void SpawnCircleRing(GameObject[] bul_pool, Vector3 origin, int bul_num=8, float rotation=0f, float speed=0f, float turnSpeed=0f, float delay=0f, float speedChangeDuration=0f, float startSpeedMod=1f, float finalSpeedMod=1f)
    {
        for (int i = 0; i < bul_num; i ++)
        {
            float angle = i * 360f / bul_num + rotation;
            Vector2 direction = RotateVector(Vector2.up, angle);
            SpawnStraightBullet(
                bul_pool, origin, direction, 
                speed:speed, delay:delay, rotation:angle, turnSpeed:turnSpeed, alignWithDir:true, 
                speedChangeDuration:speedChangeDuration, startSpeedMod:startSpeedMod, finalSpeedMod:finalSpeedMod
            );
        }
    }

    /// <summary>
    /// Given a number of bullets and a starting rotation, calls spawnMethod with calculated direction and rotation for each
    /// degree of the ring to spawn.
    /// </summary>
    /// <param name="bul_num">Number of bullets to spawn in the ring, evenly distributed in a full circle.</param>
    /// <param name="rotation">Starting rotation to begin the ring at.</param>
    /// <param name="spawnMethod">Action delegate to invoke to spawn the bullets in the ring. Given a direction and rotation, should spawn a bullet.</param>
    protected void SpawnCircleRing(int bul_num, float rotation, Action<Vector2, float> spawnMethod)
    {
        for (int i = 0; i < bul_num; i ++)
        {
            float angle = i * 360f / bul_num + rotation;
            Vector2 direction = RotateVector(Vector2.up, angle);
            spawnMethod(direction, angle);
        }
    }

    protected List<GameObject> SpawnSquareRing(GameObject[] bul_pool, Vector3 origin, int bul_num=8, float rotation=0f, float speed=0f, float speedChangeDuration=0f, float startSpeedMod=1f, float finalSpeedMod=1f, float noise=0f, float delay=0.15f)
    {
        List<GameObject> spawnedBullets = new List<GameObject>();
        for (int i = 0; i < bul_num; i ++)
        {
            float squareOffset = Mathf.Lerp(1f, -1f, i/(bul_num-1f));
            for (int side = 0; side < 4; side ++) {
                Vector2 dir = new Vector2(side%2==0 ? squareOffset : (side < 2 ? 1f : -1f), side%2!=0 ? squareOffset : (side < 2 ? 1f : -1f)).normalized;
                dir = RotateVector(dir, rotation);
                float baseSpeed = speed;
                float trueSpeed = baseSpeed * Mathf.Sqrt(1f+Mathf.Pow(Mathf.Lerp(0f, 1f, Mathf.Abs((i/((bul_num-1)/2f)) - 1f)), 2));
                spawnedBullets.Add(SpawnStraightBullet(
                    bul_pool, origin+(Vector3)Random.insideUnitCircle*noise,
                    direction:dir, speed:trueSpeed, delay:delay, 
                    rotation:Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f,
                    speedChangeDuration:speedChangeDuration, startSpeedMod:startSpeedMod, finalSpeedMod:finalSpeedMod
                ));
            }
        }
        return spawnedBullets;
    }
}
