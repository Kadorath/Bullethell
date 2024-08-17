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
    private Sprite[] letter_sprites;
    public GameObject[] circle_pool;

    protected override void InitEnemy() {
        letter_pool = gameManager.CreatePool(bul_letter, 500);
        act_letter_pool = gameManager.CreatePool(bul_act_letter, 500);
        letter_sprites = Resources.LoadAll<Sprite>("Sprites/letterbullets");
        int r = Random.Range(0,28);
        foreach (GameObject bul in letter_pool) {
            bul.GetComponent<SpriteRenderer>().sprite = letter_sprites[r];
            r = Random.Range(0,28);
        }
        circle_pool = gameManager.CreatePool(bul_circle, 1000);        
        patterns = new string[] {"Interlude2","Interlude1", "Spell1", "Spell2"};
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
        StartCoroutine("MoveTo", new Vector2(0f, 1f));
        Coroutine aux1 = StartCoroutine("Inter2_aux1");
        yield return PatternTimer(60f);
        NextPattern();
    }

    IEnumerator Inter2_aux1() {
        yield return WaitForFixedDuration(1f);
        int bul_num = 6;
        float pad = 0.25f;
        int tog = -1; 
        while (true) {
            for (int i = 1; i < bul_num; i ++) {
                SpawnActionBullet(act_letter_pool, transform.position, 
                    new Vector2(((2*(BOUND_X-pad))/bul_num)*i - (BOUND_X-pad), BOUND_Y-pad), 3f, 
                    (self)=>{
                        self.GetComponent<ActionBulletBehavior>().destination = new Vector2(tog*(BOUND_X-pad),BOUND_Y-pad);
                        self.transform.rotation = Quaternion.Euler(0f,0f,0f);
                        self.GetComponent<ActionBulletBehavior>().at_destination = (self) => {
                                float r = Random.Range(0f,2f);
                                for (int i = 0; i < 5; i ++) {
                                    SpawnStraightBullet(circle_pool, self.transform.position,
                                        new Vector2(Mathf.Cos(r+(Mathf.PI/4)*i), Mathf.Sin(r+(Mathf.PI/4)*i)), 4f, 0f,
                                        rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/4)*i), Mathf.Cos(r+(Mathf.PI/4)*i))));
                                }
                                self.GetComponent<ActionBulletBehavior>().DestroySelf();
                            };
                    }, RADTODEG*(Mathf.Atan2(BOUND_Y-pad,((2*(BOUND_X-pad))/bul_num)*i - (BOUND_X-pad))));
                SpawnActionBullet(act_letter_pool, transform.position, 
                    new Vector2((BOUND_X-pad) - ((2*(BOUND_X-pad))/bul_num)*i, -1*(BOUND_Y-pad)), 3f, 
                    (self)=>{
                        self.GetComponent<ActionBulletBehavior>().destination = new Vector2(tog*(BOUND_X-pad),-1*(BOUND_Y-pad));
                        self.transform.rotation = Quaternion.Euler(0f,0f,0f);
                        self.GetComponent<ActionBulletBehavior>().at_destination = (self) => {
                                float r = Random.Range(0f,2f);
                                for (int i = 0; i < 5; i ++) {
                                    SpawnStraightBullet(circle_pool, self.transform.position,
                                        new Vector2(Mathf.Cos(r+(Mathf.PI/4)*i), Mathf.Sin(r+(Mathf.PI/4)*i)), 4f, 0f,
                                        rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/4)*i), Mathf.Cos(r+(Mathf.PI/4)*i))));
                                }
                                self.GetComponent<ActionBulletBehavior>().DestroySelf();
                            };
                    }, RADTODEG*(Mathf.Atan2(-1*(BOUND_Y-pad), (BOUND_X-pad) - ((2*(BOUND_X-pad))/bul_num)*i)));
                yield return WaitForFixedDuration(0.5f);  
                tog *= -1;
            }
            for (int i = 0; i < bul_num; i ++) {
                SpawnActionBullet(act_letter_pool, transform.position, 
                    new Vector2(BOUND_X-pad, (BOUND_Y-pad) - ((2*(BOUND_Y-pad))/bul_num)*i), 3f, 
                    (self)=>{
                        self.GetComponent<ActionBulletBehavior>().destination = new Vector2(BOUND_X-pad,tog*(BOUND_Y-pad));
                        self.transform.rotation = Quaternion.Euler(0f,0f,90f);
                        self.GetComponent<ActionBulletBehavior>().at_destination = (self) => {
                                float r = Random.Range(0f,2f);
                                for (int i = 0; i < 5; i ++) {
                                    SpawnStraightBullet(circle_pool, self.transform.position,
                                        new Vector2(Mathf.Cos(r+(Mathf.PI/4)*i), Mathf.Sin(r+(Mathf.PI/4)*i)), 4f, 0f,
                                        rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/4)*i), Mathf.Cos(r+(Mathf.PI/4)*i))));
                                }
                                self.GetComponent<ActionBulletBehavior>().DestroySelf();
                            };
                    }, RADTODEG*(Mathf.Atan2(-1f + (BOUND_Y-pad) - ((2*(BOUND_Y-pad))/bul_num)*i,BOUND_X-pad)));   
                SpawnActionBullet(act_letter_pool, transform.position, 
                    new Vector2(-1*(BOUND_X-pad), ((2*(BOUND_Y-pad))/bul_num)*i - (BOUND_Y-pad)), 3f, 
                    (self)=>{
                        self.GetComponent<ActionBulletBehavior>().destination = new Vector2(-1*(BOUND_X-pad),tog*(BOUND_Y-pad));
                        self.transform.rotation = Quaternion.Euler(0f,0f,90f);
                        self.GetComponent<ActionBulletBehavior>().at_destination = (self) => {
                                float r = Random.Range(0f,2f);
                                for (int i = 0; i < 5; i ++) {
                                    SpawnStraightBullet(circle_pool, self.transform.position,
                                        new Vector2(Mathf.Cos(r+(Mathf.PI/4)*i), Mathf.Sin(r+(Mathf.PI/4)*i)), 4f, 0f,
                                        rotation:-90f+RADTODEG*(Mathf.Atan2(Mathf.Sin(r+(Mathf.PI/4)*i), Mathf.Cos(r+(Mathf.PI/4)*i))));
                                }
                                self.GetComponent<ActionBulletBehavior>().DestroySelf();
                            };
                    }, RADTODEG*(Mathf.Atan2(-1f + ((2*(BOUND_Y-pad))/bul_num)*i - (BOUND_Y-pad),-1*(BOUND_X-pad))));
                yield return WaitForFixedDuration(0.5f);       
                tog *= -1;
            }
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
            }
            yield return MoveBullet(camera_frame, frame_speed, player.transform.position, 0f);
        }
    }
}
