using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class NewsieBehavior : EnemyBehavior
{
    [Header("Additional Bullet Config")]
    public GameObject snapshot;
    private GameObject[] _letterPool;
    private GameObject[] _actLetterPool;
    private GameObject[] _paperballPool;
    private GameObject[] _starPool;
    private Sprite[] _actLetterSprites;
    private Sprite[] _letterSprites;

    protected override void InitEnemy() {
        _letterPool = _bulletPools["Letter"];
        _actLetterPool = _bulletPools["Act Letter"];

        _actLetterSprites = Resources.LoadAll<Sprite>("Sprites/letterbullets_orange");        
        foreach (GameObject bul in _actLetterPool) {
            int r = Random.Range(0,28);
            bul.GetComponent<BulletBehavior>().SetSprite(_actLetterSprites[r]);
        }
        _letterSprites = Resources.LoadAll<Sprite>("Sprites/letterbullets_blank");
        foreach (GameObject bul in _letterPool) {
            int r = Random.Range(0,28);
            bul.GetComponent<BulletBehavior>().SetSprite(_letterSprites[r]);
        }
        _paperballPool = _bulletPools["Paper Ball"];

        _starPool = _bulletPools["Star"];

        _patternList = new Pattern[]
        {
            new Pattern("Interlude1", 5000, 60, Inter1_aux1()),   
            new Pattern("Interlude2", 5000, 30, Inter2_aux1()),
            new Pattern("Interlude3", 5000, 60, Inter3_aux1()),
            new Pattern("Spell1", 5000, 60, Spell1_aux1(), Spell1_aux2(1), Spell1_aux3(), Spell1_aux4()),  // Sunday Special
            new Pattern("Spell2", 5000, 60, Spell2_aux1(), Spell2_aux2(), Spell2_aux3()), // Smile to the Camera
            new Pattern("Spell3", 5000, 60, Spell3_aux1()),
        };
    }

    IEnumerator Inter1_aux1() {
        yield return MoveTo(new Vector2(0f, 3f));
        int bul_num = 48;
        while (true) {
            for (int i = 0; i < bul_num; i ++) {
                float noise = Random.value * 8f;
                for (int j = 0; j < 8; j ++) {
                    float rad = j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num) + 0.1f)*i + noise;
                    SpawnStraightBullet(_paperballPool,transform.position,
                        new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)),
                        speed: 3f, delay: 0.1f,
                        rotation:-90f+(RADTODEG*(Mathf.Atan2(
                            Mathf.Sin(rad),
                            Mathf.Cos(rad))))
                        );
                }
                yield return WaitForFixedDuration(0.05f);
            }
            for (int i = bul_num; i >= 0; i -= 1) {
                for (int j = 0; j < 4; j ++) {
                    float rad = 0.07f + j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num) + 0.2f)*i;
                    SpawnStraightBullet(_starPool,transform.position,
                        new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)),
                        5f, 0.1f,
                        rotation:-90f+(RADTODEG*(Mathf.Atan2(
                            Mathf.Sin(rad), Mathf.Cos(rad)))),
                            speedChangeDuration:3f,startSpeedMod:1f,finalSpeedMod:0.3f
                        );
                }
                yield return WaitForFixedDuration(0.005f);
            }
            yield return RandomMove();
        }
    }

    IEnumerator Inter2_aux1()
    {
        yield return MoveTo(new Vector3(BOUND_X, transform.position.y+0.25f), 1f);
        yield return WaitForFixedDuration(0.5f);

        bool left = false;
        int bul_num = 48;
        while (true)
        {
            StartCoroutine(MoveTo(new Vector3(left ? BOUND_X-1f : -BOUND_X+1f, Random.Range(2f, 5f)), bul_num*0.02f));
            left = !left;
            for (int i = 0; i < bul_num; i ++)
            {
                SpawnGravityBulet(_paperballPool, 
                    origin:transform.position + (Vector3)Random.insideUnitCircle*0.5f, 
                    direction:new Vector2(Random.Range(-0.25f, 0.25f), 1.5f),
                    speed:3f,delay:0.1f,gravityScale:1.8f
                );
                if (i % 24 == 0)
                    SpawnSquareRing(_paperballPool, transform.position + (Vector3)Random.insideUnitCircle*0.4f, 6, 
                        rotation:Random.Range(0f,180f), speed:2f, 
                        speedChangeDuration: 0.2f, startSpeedMod: 3f, finalSpeedMod: 1.4f, noise:0.1f
                    );
                yield return new WaitForSeconds(0.02f);
            }

            yield return Inter2_aux2();
            yield return WaitForFixedDuration(1.5f);
        }
    }

    IEnumerator Inter2_aux2()
    {
        GameObject camera_frame = Instantiate(snapshot, transform.position, Quaternion.identity);
        camera_frame.transform.localScale = new Vector2(0.1f, 0.1f);
        GameObject poolHolder = new GameObject("Bullet Pool");
        poolHolder.transform.SetParent(camera_frame.transform);
        yield return MoveBullet(camera_frame, 3f, player.transform.position + (Vector3)Random.insideUnitCircle*0.8f, Random.Range(-12f, 12f), new Vector2(1.5f, 1.5f));
        camera_frame.GetComponent<SnapshotBehavior>().Flash();
        SpawnSquareRing(_starPool, camera_frame.transform.position, 8, 
            rotation:Random.Range(0f,180f), speed:6f, 
            speedChangeDuration: 0.8f, startSpeedMod: 1.3f, finalSpeedMod: 0.2f, noise:0.15f
        );
        yield return WaitForFixedDuration(0.06f);
        Destroy(camera_frame);
    }

    public IEnumerator Inter3_aux1()
    {
        yield return MoveTo(new Vector2(0f, 2f));
        yield return WaitForFixedDuration(1f);

        List<int> cf_order = new List<int>{0,1,2,3,4,5};
        while (true)
        {
            cf_order = cf_order.OrderBy(x => Random.value).ToList();
            foreach (int i in cf_order)
            {
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire2SFX, 0.35f);
                StartCoroutine(Inter3_auxCF(i, 6));
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.4f);

            float r = Random.Range(0f, 180f);
            // number of bullets per square side
            int bul_num = 9;
            for (int j = 0; j < 5; j ++)
            {
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire1SFX, 0.65f);
                SpawnSquareRing(
                    _paperballPool, transform.position, bul_num, r + j*5f, 2.5f,
                    speedChangeDuration:0.9f, startSpeedMod:2.5f, finalSpeedMod:0.7f,
                    noise:0.1f
                );
                yield return new WaitForSeconds(0.25f);
            }

            yield return RandomMove();
            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator Inter3_auxCF(int id, int count)
    {
        float rot = Random.Range(-12f, 12f);
        float theta = id * 2f*Mathf.PI / count;
        GameObject cf = Instantiate(snapshot, transform.position, Quaternion.identity);
        Vector2 upperOffset = new Vector2(3*Mathf.Cos(theta), 3*Mathf.Sin(theta)) + Random.insideUnitCircle * 0.05f;
        yield return MoveBullet(cf, 10f, transform.position - (Vector3)upperOffset, rot, new Vector2(1f, 1f));
        yield return new WaitForSeconds(0.8f + Random.Range(0f,0.2f));

        int bul_num = 7;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire1SFX, 0.65f);
        SpawnSquareRing(_starPool, cf.transform.position, bul_num, rotation:rot, 
            speed:1f, speedChangeDuration:0.8f, startSpeedMod:3f, finalSpeedMod:1f, noise:0.2f);

        cf.GetComponent<SnapshotBehavior>().Indicate();
        yield return new WaitForSeconds(0.1f);
        Destroy(cf);
    }

    IEnumerator Spell3_aux1()
    {
        yield return null;
    }
    // Spawn irregular rings around player at shortening intervals
    IEnumerator Spell2_aux1() 
    {
        // yield break;
        int bul_num = 18;
        float freq_mod = 0f;
        float radius = 6f;
        yield return WaitForFixedDuration(1f);
        while (true) {
            float r_x = Random.Range(-0.5f,0.5f);
            float r_y = Random.Range(-0.5f,0.5f);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire1SFX, 0.65f);
            for (int i = 0; i < bul_num; i ++) {
                int r = Random.Range(0,12);
                SpawnStraightBullet(_starPool, new Vector2(radius*Mathf.Cos(r+(Mathf.PI/bul_num/2)*i)+r_x,radius*Mathf.Sin(r+(Mathf.PI/bul_num/2)*i)+r_y),
                    new Vector2(Mathf.Cos(r+(Mathf.PI/bul_num/2)*i),
                                Mathf.Sin(r+(Mathf.PI/bul_num/2)*i)) * -1, 1.5f, 0.25f,
                    rotation:Random.Range(0f, 180f));
                yield return new WaitForSeconds(0.01f);
            }
            if(freq_mod < 1.5f) {
                freq_mod += 0.15f;
            }
            yield return WaitForFixedDuration(4f - freq_mod);
        }
    }

    // Snapshot state within frame, wait, and then restore that state as it was captured
    IEnumerator Spell2_aux2() {
        GameObject camera_frame = Instantiate(snapshot, new Vector3(0f,.25f), Quaternion.identity);
        camera_frame.transform.localScale = new Vector2(12f, 12f);
        SnapshotBehavior cf_behavior = camera_frame.GetComponent<SnapshotBehavior>();
        cf_behavior.lock_player = true;
        float frame_speed = 5f;
        float center_x = 0f;
        float center_y = -0.5f;
        yield return MoveBullet(camera_frame, frame_speed, new Vector3(center_x,center_y, -1f), 0f, new Vector2(2.5f,2.5f));
        yield return WaitForFixedDuration(1f);
        float noise_x = 0f;
        float noise_y = 0f;
        while (true) {
            noise_x = Random.Range(-1f, 1f);
            noise_y = Random.Range(-2f, -1f);
            yield return MoveBullet(camera_frame, frame_speed, new Vector3(center_x+noise_x, center_y+noise_y, -1f));
            cf_behavior.Snapshot();
            yield return WaitForFixedDuration(1f + Random.Range(0.5f, 3f));
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.5f);
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.4f);
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.25f);
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.12f);
            cf_behavior.DevelopCapture();
            yield return WaitForFixedDuration(1f + Random.Range(0f, 2.5f));
        }
    }

    // Fire messy square rings at random intervals
    IEnumerator Spell2_aux3()
    {
        yield return WaitForFixedDuration(0.5f);
        SpawnSquareRing(_paperballPool, transform.position, 8, 
            rotation:Random.Range(0f, 180f), speed:3f,
            speedChangeDuration:0.45f, startSpeedMod:2f, finalSpeedMod:0.6f,
            noise:0.1f
        );
        yield return WaitForFixedDuration(1.5f);
        while (true)
        {
            StartCoroutine(RandomMove());
            int waves = Random.Range(2, 6);
            for (int i = 0; i < waves; i ++)
            {
                SpawnSquareRing(_paperballPool, transform.position, 6, 
                    rotation:Random.Range(0f, 180f), speed:3f,
                    speedChangeDuration:0.45f, startSpeedMod:2f, finalSpeedMod:0.6f,
                    noise:0.1f
                );
                yield return WaitForFixedDuration(1f/waves);
            }
            yield return WaitForFixedDuration(Random.Range(0.75f,1.5f));
        }
    }

    
    // Launch squares of bullets at regular intervals, send lines of letter bullets horizontally across the screen
    IEnumerator Spell1_aux1() {
        yield return WaitForFixedDuration(1f);
        while (true) {
            yield return WaitForFixedDuration(.07f);
            for (int i = 0; i < 6; i ++) {
                SpawnStraightBullet(_letterPool, new Vector2(-6.25f, 5f-(12*i/6f)),
                    Vector2.right, speed:7f, alignWithDir:false, rotation:0f);
            }
        }
    }

    // Movement of snapshot frames in Spell1
    IEnumerator Spell1_aux2(int id) {
        yield return WaitForFixedDuration(4f);

        GameObject camera_frame = Instantiate(snapshot, transform.position, Quaternion.identity);
        SnapshotBehavior cf_behavior = camera_frame.GetComponent<SnapshotBehavior>();
        float frame_speed = 4f;
        Collider2D[] captured_bullets;

        float s = id == 1 ? 2.5f : 1.5f;
        Vector2 startMoveTarget = id == 1 ? player.transform.position : new Vector2(Random.Range(-1*(BOUND_X-2f),BOUND_X-2f),Random.Range(2f, 4f));
        yield return MoveBullet(camera_frame, frame_speed, startMoveTarget, 0f, new Vector2(s,s));

        int ct = 0;
        while (true) {
            yield return WaitForFixedDuration(0.7f + id==3?0.25f:0f);
            captured_bullets = cf_behavior.Snapshot();

            cf_behavior.FreezeFrame();

            yield return WaitForFixedDuration(0.25f);

            ct ++;
            if (ct == 2 && id == 1)
                StartCoroutine("Spell1_aux2", 2);
            if (ct == 3 && id == 1)
                StartCoroutine("Spell1_aux2", 3);

            Vector2 target_pos = new Vector2(2f, 4f);
            if (Random.value < 0.5f) {
                target_pos = new Vector2(-1*target_pos.x, target_pos.y);
            }

            float target_angle = RADTODEG*angleToPlayer(target_pos);
            yield return MoveBullet(camera_frame, frame_speed, target_pos, target_angle);

            yield return WaitForFixedDuration(0.05f);

            cf_behavior.Indicate();
            cf_behavior.UnfreezeFrame();

            yield return WaitForFixedDuration(0.15f);
            
            if (id == 1) {
                yield return RandomMove();
                yield return MoveBullet(camera_frame, frame_speed, player.transform.position, 0f);
            }
            else {
                yield return MoveBullet(camera_frame, frame_speed, 
                    new Vector2(Random.Range(-1*(BOUND_X-2f),BOUND_X-2f),Random.Range(2f, 4f)), 
                    0f);
            }
        }
    }

    IEnumerator Spell1_aux3() {
        yield return WaitForFixedDuration(1f);
        while (true) {
            int r = Random.Range(2, 5);
            List<float> lineYs = new List<float>();
            for (int i = 0; i < r; i ++)
                lineYs.Add(player.transform.position.y + Random.Range(-1f, 1f));
            
            for (int i = 0; i < lineYs.Count; i ++)
            {
                StartCoroutine(Spell1_aux2_1(lineYs[i], i%2 == 0));
                yield return WaitForFixedDuration(0.5f);
            }

            yield return WaitForFixedDuration(2.5f);
        }
    }

    // Square ring of paper balls
    IEnumerator Spell1_aux4()
    {
        while (true)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire1SFX, 0.65f);
            SpawnSquareRing(_paperballPool, transform.position, 14, Random.Range(0, 180f), 3f, 0.45f, 2f, 0.6f, 0.1f);
            yield return WaitForFixedDuration(2f);
        }
    }

    IEnumerator Spell1_aux2_1(float y, bool right) {
        for (int i = 0; i < 8; i ++)
        {
            SpawnStraightBullet(_letterPool, new Vector2(6.25f * (right ? -1 : 1), y),
                right ? Vector2.right : Vector2.left, 
                speed:6f, alignWithDir:false, rotation:0f);
            yield return WaitForFixedDuration(.075f);
        }
    }

    // Send bullets in a spiral out towards the camera frame
    IEnumerator Spin_aux1() {
        yield return WaitForFixedDuration(1f);
        GameObject camera_frame = Instantiate(snapshot, new Vector3(0f,2f), Quaternion.identity);
        camera_frame.transform.SetParent(transform);
        foreach (GameObject bul in _actLetterPool) {
            bul.transform.SetParent(camera_frame.transform, true);
        }
        float frame_size = 1f;
        yield return MoveBullet(camera_frame, 10f, new Vector3(0f,1.5f,-1f), 0f, new Vector2(frame_size,frame_size));
        StartCoroutine("Inter2_aux2", camera_frame);
        int bul_num = 6;

        GameObject[] target_pts = new GameObject[bul_num*4];
        for (int i = 0; i < bul_num*4; i ++) {
            GameObject target_pt = new GameObject("TarPt" + i);
            target_pt.transform.SetParent(camera_frame.transform, false);
            int side = i/bul_num;
            if (side == 0) {
                target_pt.transform.localPosition = new Vector3(1f, (2f/bul_num)*i - 1f, 0f);
            }
            else if (side == 1) {
                target_pt.transform.localPosition = new Vector3(1f - (2f/bul_num)*(i%bul_num), 1f, 0f);
            }
            else if (side == 2) {
                target_pt.transform.localPosition = new Vector3(-1f, 1f - (2f/bul_num)*(i%bul_num), 0f);
            }
            else if (side == 3) {
                target_pt.transform.localPosition = new Vector3((2f/bul_num)*(i%bul_num) - 1f, -1f, 0f);
            }
            target_pts[i] = target_pt;
        }

        StartCoroutine("Inter2_aux3", camera_frame);

        int ct = 0;
        while (true) {
            for (int i = 0; i < bul_num * 4; i++)
            {
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire3SFX, 0.25f);
                SpawnActionBullet(_actLetterPool, transform.position,
                    target_pts[i].transform.localPosition, .3f,
                    (self) =>
                    {
                        SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellBreakSFX, 0.4f);
                        float r = Random.Range(0f, Mathf.PI);
                        for (int i = 0; i < 4; i++)
                        {
                            SpawnStraightBullet(_starPool, self.transform.position,
                                new Vector2(Mathf.Cos(r + (Mathf.PI / 2) * i), Mathf.Sin(r + (Mathf.PI / 2) * i)), Random.Range(2f, 4f), .05f,
                                rotation: -90f + RADTODEG * (Mathf.Atan2(Mathf.Sin(r + (Mathf.PI / 2) * i), Mathf.Cos(r + (Mathf.PI / 2) * i))));
                        }
                        self.GetComponent<ActionBulletBehavior>().DestroySelf();
                    }, camera_frame.transform.eulerAngles.z + RADTODEG * (Mathf.Atan2(target_pts[i].transform.localPosition.y, target_pts[i].transform.localPosition.x)),
                .15f);
                int i_two = (i + (bul_num / 2)) % (bul_num * 4);
                SpawnActionBullet(_actLetterPool, transform.position,
                    target_pts[i_two].transform.localPosition, .3f,
                    (self) =>
                    {
                        SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellBreakSFX, 0.55f);
                        float r = Random.Range(0f, Mathf.PI);
                        for (int i = 0; i < 4; i++)
                        {
                            SpawnStraightBullet(_starPool, self.transform.position,
                                new Vector2(Mathf.Cos(r + (Mathf.PI / 2) * i), Mathf.Sin(r + (Mathf.PI / 2) * i)), Random.Range(2f, 4f), .05f,
                                rotation: -90f + RADTODEG * (Mathf.Atan2(Mathf.Sin(r + (Mathf.PI / 2) * i), Mathf.Cos(r + (Mathf.PI / 2) * i))));
                        }
                        self.GetComponent<ActionBulletBehavior>().DestroySelf();
                    }, camera_frame.transform.eulerAngles.z + RADTODEG * (Mathf.Atan2(target_pts[i_two].transform.localPosition.y, target_pts[i_two].transform.localPosition.x)),
                .15f);
                yield return WaitForFixedDuration(.15f);
            }
            ct += 1;
            if (ct == 2) {
                yield return RandomMove();
                ct = 0;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.spellFire1SFX, 0.65f);
                for (int i = 0; i < 24; i ++) {
                    SpawnActionBullet(_actLetterPool, transform.position, 
                        target_pts[i].transform.localPosition, 3f, 
                        (self)=>{
                            float r = Random.Range(0f,Mathf.PI);
                            for (int i = 0; i < 4; i ++) {
                                SpawnStraightBullet(_starPool, self.transform.position,
                                    new Vector2(Mathf.Cos(r+(Mathf.PI/2)*i), Mathf.Sin(r+(Mathf.PI/2)*i)), Random.Range(2f,4f), .05f,
                                    rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/2)*i), Mathf.Cos(r+(Mathf.PI/2)*i))));
                            }
                            self.GetComponent<ActionBulletBehavior>().DestroySelf();
                        }, camera_frame.transform.eulerAngles.z + RADTODEG*(Mathf.Atan2(target_pts[i].transform.localPosition.y,target_pts[i].transform.localPosition.x)),
                    .1f);
                }
                camera_frame.GetComponent<SnapshotBehavior>().Indicate();
                yield return WaitForFixedDuration(0.075f);
            }            
        }
    }

    // Rotate the frame
    IEnumerator Spin_aux2(GameObject cf) {
        float elapsed = 0f;
        while (true) {
            cf.transform.Rotate(0f,0f, 0.4f * Mathf.Sin(elapsed/4f));
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    // Slowly increase the frame's size
    IEnumerator Spin_aux3(GameObject cf)
    {
        float frame_grow_speed = 0.04f;
        while (true)
        {
            Vector3 s = cf.transform.localScale;
            if (s.x > 2f) break;

            float grow_delta = frame_grow_speed * Time.fixedDeltaTime;
            cf.transform.localScale = new Vector3(s.x + grow_delta, s.y + grow_delta, s.z);

            foreach (Transform child in cf.transform)
            {
                if (child.CompareTag("Bullet") && child.GetComponent<BulletBehavior>().IsSpawned())
                    child.localScale = new Vector3(1f / cf.transform.localScale.x, 1f / cf.transform.localScale.y, child.localScale.z);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

    // IEnumerator Spell3_aux1()
    // {
    //     yield return WaitForFixedDuration(1f);
    //     GameObject camera_frame = Instantiate(snapshot, new Vector3(0f, 0.1f), Quaternion.identity);
    //     GameObject poolHolder = new GameObject("Bullet Pool");
    //     poolHolder.transform.SetParent(camera_frame.transform);
    //     foreach (GameObject bul in _actLetterPool)
    //     {
    //         bul.transform.SetParent(poolHolder.transform, true);
    //     }
    //     float frame_size = 4.8f;
    //     yield return MoveBullet(camera_frame, 3f, new Vector3(0f, 0f, -1f), 0f, new Vector2(frame_size, frame_size));
    //     camera_frame.GetComponent<SnapshotBehavior>().lock_player = true;
    //     player.transform.SetParent(camera_frame.transform);
    //     transform.SetParent(camera_frame.transform);

    //     yield return new WaitForSeconds(1f);
    //     StartCoroutine("Spell3_aux2", camera_frame);
    // }

    // Snapshot Frame reflection
    // IEnumerator Spell3_aux2(GameObject cf)
    // {
    //     float frame_flip_time = 12f;
    //     while (true)
    //     {
    //         // Flip the frame
    //         cf.GetComponent<SnapshotBehavior>().SnapshotImageOnly();
    //         yield return new WaitForSeconds(0.5f);
    //         Vector3 s = cf.transform.localScale;
    //         float targetDim = -s.x;
    //         cf.GetComponent<SnapshotBehavior>().lock_player = false;
    //         player.GetComponent<PlayerBehavior>().invulnerable = true;
    //         // I don't know why this is necessary. Something is setting the Player's
    //         // local position to Vector3.zero on the inflection point (localScale of 0
    //         // destroys local position information, perhaps?)
    //         Vector3 original_pos = player.transform.localPosition;
    //         for (int i = 1; i <= frame_flip_time; i++)
    //         {
    //             cf.transform.localScale = new Vector3(Mathf.Lerp(s.x, targetDim, i / frame_flip_time), s.y, s.z);
    //             player.transform.localPosition = original_pos;
    //             yield return new WaitForFixedUpdate();
    //         }
    //         player.GetComponent<PlayerBehavior>().invulnerable = false;
    //         cf.GetComponent<SnapshotBehavior>().lock_player = true;
    //         yield return new WaitForSeconds(6f);

    //         // Revert back to original scale
    //         cf.GetComponent<SnapshotBehavior>().ClearImage();
    //         cf.GetComponent<SnapshotBehavior>().lock_player = false;
    //         player.GetComponent<PlayerBehavior>().invulnerable = true;
    //         original_pos = player.transform.localPosition;
    //         for (int i = 1; i <= frame_flip_time; i++)
    //         {
    //             cf.transform.localScale = new Vector3(Mathf.Lerp(targetDim, s.x, i / frame_flip_time), s.y, s.z);
    //             player.transform.localPosition = original_pos;
    //             yield return new WaitForFixedUpdate();
    //         }
    //         player.GetComponent<PlayerBehavior>().invulnerable = false;
    //         cf.GetComponent<SnapshotBehavior>().lock_player = true;
    //         yield return new WaitForSeconds(12f);
    //     }
    // }
