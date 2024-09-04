using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsieBehavior : EnemyBehavior
{
    public GameObject bul_letter;
    public GameObject bul_act_letter;
    public GameObject bul_circle;
    public GameObject snapshot;
    public GameObject[] letter_pool;
    public GameObject[] act_letter_pool;
    private Sprite[] act_letter_sprites;
    private Sprite[] letter_sprites;
    public GameObject[] circle_pool;

    protected override void InitEnemy() {
        letter_pool = gameManager.CreatePool(bul_letter, 500);
        act_letter_pool = gameManager.CreatePool(bul_act_letter, 500);

        act_letter_sprites = Resources.LoadAll<Sprite>("Sprites/letterbullets_orange");        
        foreach (GameObject bul in act_letter_pool) {
            int r = Random.Range(0,28);
            bul.GetComponent<SpriteRenderer>().sprite = act_letter_sprites[r];
        }
        letter_sprites = Resources.LoadAll<Sprite>("Sprites/letterbullets");
        foreach (GameObject bul in letter_pool) {
            int r = Random.Range(0,28);
            bul.GetComponent<SpriteRenderer>().sprite = letter_sprites[r];
        }
        circle_pool = gameManager.CreatePool(bul_circle, 1000);        
        patterns = new string[] {"Spell3", "Interlude1", "Spell1", "Interlude2", "Spell2"};
    }

    IEnumerator Interlude1() {
        yield return WaitForFixedDuration(2f);
        health = 2500;
        maxhealth = 2500;
        Coroutine aux1 = StartCoroutine("Inter1_aux1");
        yield return PatternTimer(60f);
        NextPattern();
    }

    IEnumerator Inter1_aux1() {
        int bul_num = 48;
        while (true) {
            for (int i = 0; i < bul_num; i ++) {
                for (int j = 0; j < 8; j ++) {
                SpawnStraightBullet(circle_pool,transform.position,
                    new Vector2(Mathf.Cos(j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num) + 0.1f)*i),
                                    Mathf.Sin(j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num) + 0.1f)*i)),
                    3f, 0.1f,
                    rotation:-90f+(RADTODEG*(Mathf.Atan2(
                        Mathf.Sin(j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num) + 0.1f)*i),
                        Mathf.Cos(j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num) + 0.1f)*i))))
                    );
                }
                yield return WaitForFixedDuration(0.05f);
            }
            for (int i = bul_num/2; i >= 0; i -= 1) {
                for (int j = 0; j < 8; j ++) {
                SpawnStraightBullet(circle_pool,transform.position,
                    new Vector2(Mathf.Cos(0.07f + j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num))*i),
                                    Mathf.Sin(0.07f + j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num))*i)),
                    6f, 0.1f,
                    rotation:-90f+(RADTODEG*(Mathf.Atan2(
                        Mathf.Sin(0.07f + j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num))*i),
                        Mathf.Cos(0.07f + j*Mathf.PI/4 + ((2*Mathf.PI)/(bul_num))*i))))
                    );
                }
                yield return WaitForFixedDuration(0.005f);
            }
            yield return RandomMove();
        }
    }

    IEnumerator Interlude2() {
        yield return WaitForFixedDuration(1f);
        health = 2500;
        maxhealth = 2500;
        StartCoroutine("MoveTo", new Vector2(0f, 2f));
        Coroutine aux1 = StartCoroutine("Inter2_aux1");
        yield return PatternTimer(60f);
        NextPattern();
    }

    IEnumerator Inter2_aux1() {
        yield return WaitForFixedDuration(2f);
        GameObject camera_frame = Instantiate(snapshot, new Vector3(0f,2f), Quaternion.identity);
        camera_frame.transform.SetParent(transform);
        foreach (GameObject bul in act_letter_pool) {
            bul.transform.SetParent(camera_frame.transform, true);
        }
        float frame_size = 1f;
        yield return MoveBullet(camera_frame, 8f, new Vector3(0f,1.5f,-1f), 0f, new Vector2(frame_size,frame_size));
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

        int ct = 0;
        while (true) {
            for (int i = 0; i < bul_num*4; i ++) {
                SpawnActionBullet(act_letter_pool, transform.position, 
                    target_pts[i].transform.localPosition, .3f, 
                    (self)=>{
                        float r = Random.Range(0f,Mathf.PI);
                        for (int i = 0; i < 4; i ++) {
                            SpawnStraightBullet(circle_pool, self.transform.position,
                                new Vector2(Mathf.Cos(r+(Mathf.PI/2)*i), Mathf.Sin(r+(Mathf.PI/2)*i)), Random.Range(2f,4f), .05f,
                                rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/2)*i), Mathf.Cos(r+(Mathf.PI/2)*i))));
                        }
                        self.GetComponent<ActionBulletBehavior>().DestroySelf();
                    }, camera_frame.transform.eulerAngles.z + RADTODEG*(Mathf.Atan2(target_pts[i].transform.localPosition.y,target_pts[i].transform.localPosition.x)),
                .25f);
                int i_two = (i+(bul_num/2)) % (bul_num*4);
                SpawnActionBullet(act_letter_pool, transform.position, 
                    target_pts[i_two].transform.localPosition, .3f, 
                    (self)=>{
                        float r = Random.Range(0f,Mathf.PI);
                        for (int i = 0; i < 4; i ++) {
                            SpawnStraightBullet(circle_pool, self.transform.position,
                                new Vector2(Mathf.Cos(r+(Mathf.PI/2)*i), Mathf.Sin(r+(Mathf.PI/2)*i)), Random.Range(2f,4f), .05f,
                                rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/2)*i), Mathf.Cos(r+(Mathf.PI/2)*i))));
                        }
                        self.GetComponent<ActionBulletBehavior>().DestroySelf();
                    }, camera_frame.transform.eulerAngles.z + RADTODEG*(Mathf.Atan2(target_pts[i_two].transform.localPosition.y,target_pts[i_two].transform.localPosition.x)),
                .25f);
                yield return WaitForFixedDuration(.2f);
            }
            ct += 1;
            if (ct == 2) {
                yield return RandomMove();
                ct = 0;
                for (int i = 0; i < 24; i ++) {
                    SpawnActionBullet(act_letter_pool, transform.position, 
                        target_pts[i].transform.localPosition, 3f, 
                        (self)=>{
                            float r = Random.Range(0f,Mathf.PI);
                            for (int i = 0; i < 4; i ++) {
                                SpawnStraightBullet(circle_pool, self.transform.position,
                                    new Vector2(Mathf.Cos(r+(Mathf.PI/2)*i), Mathf.Sin(r+(Mathf.PI/2)*i)), Random.Range(2f,4f), .05f,
                                    rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/2)*i), Mathf.Cos(r+(Mathf.PI/2)*i))));
                            }
                            self.GetComponent<ActionBulletBehavior>().DestroySelf();
                        }, camera_frame.transform.eulerAngles.z + RADTODEG*(Mathf.Atan2(target_pts[i].transform.localPosition.y,target_pts[i].transform.localPosition.x)),
                    .1f);
                }
                yield return WaitForFixedDuration(0.15f);
            }            
        }
    }

    IEnumerator Inter2_aux2(GameObject cf) {
        while (true) {
            cf.transform.Rotate(0f,0f,0.1f);
            yield return WaitForFixedDuration(12f);
            for (int i = 0; i < 9; i ++) {
                cf.transform.Rotate(0f,0f,5f);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    IEnumerator Spell3() {
        yield return WaitForFixedDuration(1f);
        health = 2500;
        maxhealth = 2500;
        StartCoroutine("MoveTo", new Vector2(0f, 4f));
        Coroutine aux1 = StartCoroutine("Spell3_aux1");
        yield return PatternTimer(60f);
        NextPattern();        
    }

    IEnumerator Spell3_aux1() {
        yield return WaitForFixedDuration(1f);
        while(true) {
            for (int i = 0; i < 8; i ++) {
                for (int j = 0; j < 8; j ++) {
                    SpawnStraightBullet(letter_pool, new Vector2(transform.position.x + ((1f/8)*j - .5f), transform.position.y),
                        player.transform.position - transform.position, 1f + 0.05f*(i+1), delay:0.25f, 
                        rotation:RADTODEG*angleToPlayer(transform.position));
                }
            }
            yield return WaitForFixedDuration(2f);
        }
    }

    IEnumerator Spell2() {
        yield return WaitForFixedDuration(1f);
        health = 2500;
        maxhealth = 2500;
        StartCoroutine("MoveTo", new Vector2(0f, 4f));
        Coroutine aux1 = StartCoroutine("Spell2_aux1");
        Coroutine aux2 = StartCoroutine("Spell2_aux2");
        yield return PatternTimer(60f);
        NextPattern();
    }

    IEnumerator Spell2_aux1() {
        int bul_num = 18;
        float freq_mod = 0f;
        yield return WaitForFixedDuration(1f);
        while (true) {
            float r_x = Random.Range(-0.5f,0.5f);
            float r_y = Random.Range(-0.5f,0.5f);
            for (int i = 0; i < bul_num; i ++) {
                int r = Random.Range(0,12);
                SpawnStraightBullet(circle_pool, new Vector2(6*Mathf.Cos(r+(Mathf.PI/bul_num/2)*i)+r_x,6*Mathf.Sin(r+(Mathf.PI/bul_num/2)*i)+r_y),
                    new Vector2(Mathf.Cos(r+(Mathf.PI/bul_num/2)*i),
                                Mathf.Sin(r+(Mathf.PI/bul_num/2)*i)) * -1, 1.5f, 0.25f,
                    rotation:-270f+(RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/bul_num/2)*i),Mathf.Cos(r+(Mathf.PI/bul_num/2)*i)))));
            }
            if(freq_mod < 2.5f) {
                freq_mod += 0.15f;
            }
            yield return WaitForFixedDuration(4f - freq_mod);
        }
    }

    IEnumerator Spell2_aux2() {
        GameObject camera_frame = Instantiate(snapshot, new Vector3(0f,.25f), Quaternion.identity);
        camera_frame.transform.localScale = new Vector2(12f, 12f);
        SnapshotBehavior cf_behavior = camera_frame.GetComponent<SnapshotBehavior>();
        cf_behavior.lock_player = true;
        float frame_speed = 5f;
        float center_x = 0f;
        float center_y = -0.5f;
        yield return MoveBullet(camera_frame, frame_speed, new Vector3(center_x,center_y, -1f), 0f, new Vector2(2f,2f));
        yield return WaitForFixedDuration(2f);
        float noise_x = 0f;
        float noise_y = 0f;
        while (true) {
            yield return WaitForFixedDuration(2.5f);
            cf_behavior.Snapshot();
            yield return WaitForFixedDuration(1f);
            noise_x = Random.Range(-0.25f, 0.25f);
            noise_y = Random.Range(-0.25f, 0.25f);
            yield return MoveBullet(camera_frame, frame_speed, new Vector3(center_x+noise_x, center_y+noise_y, -1f));
            yield return WaitForFixedDuration(1f);
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.5f);
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.4f);
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.25f);
            cf_behavior.Indicate();
            yield return WaitForFixedDuration(0.15f);
            yield return MoveBullet(camera_frame, frame_speed, new Vector3(center_x, center_y, -1f));
            cf_behavior.DevelopCapture();
            yield return WaitForFixedDuration(2f);
        }
    }

    IEnumerator Spell1() {
        health = 2500;
        maxhealth = 2500;
        StartCoroutine("MoveTo", new Vector2(0f, 3f));
        Coroutine aux1 = StartCoroutine("Spell1_aux1");
        Coroutine aux2 = StartCoroutine("Spell1_aux2", 1);
        yield return PatternTimer(60f);
        NextPattern();
    }
    
    IEnumerator Spell1_aux1() {
        yield return WaitForFixedDuration(1f);
        int ct = 0;
        while (true) {
            yield return WaitForFixedDuration(.25f);
            for (int i = 0; i < 12; i ++) {
                SpawnStraightBullet(letter_pool, new Vector2(-6.25f, 6f-(12f/12f)*i),
                    Vector2.right, 2f, rotation:0f);
            }
            if (ct % 5 == 0) {
                for (int i = 0; i < 32; i ++) {
                    int r_x = Random.Range(0,360);
                    int r_y = Random.Range(0,360);
                    SpawnStraightBullet(circle_pool, transform.position,
                        new Vector2(Mathf.Cos(r_x+(Mathf.PI/16)*i),
                            Mathf.Sin(r_y+(Mathf.PI/16)*i)), 3f, 0.25f, 
                            rotation:-90f+(RADTODEG*(Mathf.Atan2(Mathf.Sin(r_y+(Mathf.PI/16)*i),Mathf.Cos(r_x+(Mathf.PI/16)*i)))));
                }
            }
            ct ++;
            if (ct == 50) { ct = 0; }
        }
    }

    IEnumerator Spell1_aux2(int id) {
        yield return WaitForFixedDuration(4f);

        GameObject camera_frame = Instantiate(snapshot, transform.position, Quaternion.identity);
        SnapshotBehavior cf_behavior = camera_frame.GetComponent<SnapshotBehavior>();
        float frame_speed = 7f;
        Collider2D[] captured_bullets;

        yield return MoveBullet(camera_frame, frame_speed, player.transform.position, 0f, new Vector2(2f,2f));

        int ct = 0;
        while (true) {
            yield return WaitForFixedDuration(2f);
            captured_bullets = cf_behavior.Snapshot();

            cf_behavior.FreezeFrame();

            yield return WaitForFixedDuration(0.5f);

            ct ++;
            if (ct == 3 && id == 1) {
                StartCoroutine("Spell1_aux2", 2);
            }

            Vector2 target_pos = new Vector2(2f, 4f);
            if (Random.value < 0.5f) {
                target_pos = new Vector2(-1*target_pos.x, target_pos.y);
            }

            float target_angle = RADTODEG*angleToPlayer(target_pos);
            yield return MoveBullet(camera_frame, frame_speed, target_pos, target_angle);

            yield return WaitForFixedDuration(0.25f);

            cf_behavior.UnfreezeFrame();

            yield return WaitForFixedDuration(0.25f);
            
            if (id == 1) {
                yield return RandomMove();
                yield return MoveBullet(camera_frame, frame_speed, player.transform.position, 0f);
            }
            else {
                yield return MoveBullet(camera_frame, frame_speed, 
                    new Vector2(Random.Range(-1*(BOUND_X-2f),BOUND_X-2f),Random.Range(-1*(BOUND_Y-2f),BOUND_Y-2f)), 
                    0f);
            }
        }
    }
}
