using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownpieceSketchBehavior : EnemyBehavior
{
    public GameObject bul_star_red;
    public GameObject bul_star_blue;
    public GameObject bul_action;
    public GameObject bul_circle;
    public GameObject bul_moon;
    public GameObject laser;
    public GameObject[] red_star_pool;
    public GameObject[] blue_star_pool;
    public GameObject[] circle_pool;

    protected override void InitEnemy() {
        red_star_pool = gameManager.CreatePool(bul_star_red, 150);
        blue_star_pool = gameManager.CreatePool(bul_star_blue, 100);
        circle_pool = gameManager.CreatePool(bul_circle, 500);
        
        patterns = new string[] {"Pattern1", "HellEclipse"};
    }

    IEnumerator Pattern1() {
        health = 2500;
        maxhealth = health;
        Coroutine aux1 = StartCoroutine("Pattern1_aux1");
        Coroutine aux2 = StartCoroutine("Pattern1_aux2");
        yield return PatternTimer(40.0f);
        NextPattern();
    }
    // Spray of red stars in an arc targeted at player
    IEnumerator Pattern1_aux1() {
        int i = 0;
        bool left = false;
        float center = 90f + ((180/Mathf.PI)*angleToPlayer(transform.position));
        while (true) {
            SpawnStraightBullet(red_star_pool, transform.position,
             new Vector2(Mathf.Cos((Mathf.PI/180)*(i*10 - center)),Mathf.Sin((Mathf.PI/180)*(i*10 - center))), 5f, 1f, true);
            yield return WaitForFixedDuration(0.01f);
            
            if (left) { i -= 1; }
            else { i += 1; }
            if (i == 18 || i == 0) { 
                center = 90f + ((180/Mathf.PI) * angleToPlayer(transform.position));
                left = !left;
            }
        }
    }
    // Wings of downward lasers centered on enemy
    IEnumerator Pattern1_aux2() {
        while (true) {
            for (int i = 6; i >= 1; i --) {
                SpawnActionBullet(bul_action, transform.position,
                    new Vector2(transform.position.x+.25f+(i*.35f),transform.position.y+2f+(Mathf.Log(i)*.35f)), 6f,
                    (self) => {
                        SpawnLaser(laser, self.transform.position, 
                            new Vector2(self.transform.position.x, self.transform.position.y-1f), .2f, .75f, 1f,
                            source:self, destroy_source:true);
                    });
                SpawnActionBullet(bul_action, transform.position,
                    new Vector2(transform.position.x-.25f-(i*.35f),transform.position.y+2f+(Mathf.Log(i)*.35f)), 6f,
                    (self) => {
                        SpawnLaser(laser, self.transform.position, 
                            new Vector2(self.transform.position.x, self.transform.position.y-1f), .2f, .75f, 1f,
                            source:self, destroy_source:true);
                    });
                yield return WaitForFixedDuration(.2f);
            }
            yield return WaitForFixedDuration(4f);
            yield return RandomMove();
        }
    }

    IEnumerator HellEclipse() {
        // temp yield for loading
        yield return WaitForFixedDuration(1f);
        health = -1;
        maxhealth = health;
        StartCoroutine("MoveTo", new Vector2(0f,1f));
        Coroutine aux1 = StartCoroutine("HellEclipse_aux1");
        Coroutine aux2 = StartCoroutine("HellEclipse_aux2");
        yield return PatternTimer(30f);
        NextPattern();
    }
    // Circles of blue stars after first 9.5 seconds, every 2 seconds
    IEnumerator HellEclipse_aux1() {
        yield return WaitForFixedDuration(9.5f);
        while (true) {
            for (int i = 0; i < 32; i ++) {
                SpawnStraightBullet(blue_star_pool, transform.position,
                    new Vector2(Mathf.Cos((Mathf.PI/16)*i),Mathf.Sin((Mathf.PI/16)*i)), 3f, 1f, spin:true);
            }
            yield return WaitForFixedDuration(2f);
        }
    }

    IEnumerator HellEclipse_aux2() {
        yield return WaitForFixedDuration(.75f);
        Vector2 source = transform.position;
        GameObject moon = Instantiate(bul_moon, transform.position, Quaternion.identity);
        moon.name = "bul moon";
        float source_dist = 0f;
        float moon_dist = 0f;
        int source_move_speed = 384;
        int moon_move_speed = 512;
        int source_rot_speed = 1024;
        int w = 0;
        while (true) {
            moon.transform.position = new Vector2(moon_dist*Mathf.Cos((Mathf.PI*2)-(Mathf.PI/(moon_move_speed/2))*w),
                transform.position.y+moon_dist*Mathf.Sin((Mathf.PI*2)-(Mathf.PI/(moon_move_speed/2))*w));
            if (moon_dist < 3f) { moon_dist += 0.04f; }
            else { break; }
            w ++;
            if (w == moon_move_speed) { w = 0; }
            yield return WaitForFixedDuration(0.01f);
        }
        int i = 0;
        int k = 0;
        while (true) {
            moon.transform.position = new Vector2(moon_dist*Mathf.Cos((Mathf.PI*2)-(Mathf.PI/(moon_move_speed/2))*w),
                transform.position.y+moon_dist*Mathf.Sin((Mathf.PI*2)-(Mathf.PI/(moon_move_speed/2))*w));
            source = new Vector2(source_dist*Mathf.Cos((Mathf.PI/(source_move_speed/2))*i),
                transform.position.y+source_dist*Mathf.Sin((Mathf.PI/(source_move_speed/2))*i));
            for (int j = 0; j < 12; j ++) {
                SpawnStraightBullet(circle_pool, source, 
                    new Vector2(Mathf.Cos((Mathf.PI/(source_rot_speed/2))*k + (Mathf.PI/6)*j),
                        Mathf.Sin((Mathf.PI/(source_rot_speed/2))*k + (Mathf.PI/6)*j)), 10f, 1f);
            }

            if (source_dist < 2f) { source_dist += 0.01f; }

            yield return WaitForFixedDuration(0.01f);
            i ++; w ++; k ++;
            if(i == source_move_speed) { i = 0; }
            if (k == source_rot_speed) { k = 0; }
            if (w == moon_move_speed) { w = 0; }
        }
    }  
    

    IEnumerator TestAttack() {
        yield return new WaitForSeconds(1f);
        int i = 0;
        while (true) {
            SpawnStraightBullet(red_star_pool, transform.position,
             new Vector2(Mathf.Cos((Mathf.PI*2 / 30)*i),Mathf.Sin((Mathf.PI*2 / 30)*i)), 5f);
            yield return WaitForFixedDuration(0.05f);
            i += 1;
            if(i == 30) { 
                GameObject act_bul = SpawnActionBullet(bul_action, 
                    transform.position, 
                    new Vector2(0,0), 
                    2f, 
                    (self) => { 
                        SpawnLaser(laser, new Vector2(0,0), player.transform.position); 
                        Destroy(self);
                    });
                // SpawnLaser(laser, transform.position, player.transform.position);
                i = 0; 
            }
        }
    }
}
