using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHEntity : MonoBehaviour
{
    public const float RADTODEG = 180/Mathf.PI;
    public const float DEGTORAD = Mathf.PI/180;
    public const float BOUND_X = 4.8f;
    public const float BOUND_Y = 6f;

    protected GameObject FindNextBullet(GameObject[] pool) {
        for (int i = 0; i < pool.Length; i ++) {
            if (!pool[i].activeSelf) {
                pool[i].GetComponent<BulletBehavior>().ResetGraze();   
                return pool[i];
            }
        }
        return null;
    }

    protected GameObject SpawnStraightBullet(GameObject[] pool, Vector2 pos, Vector2 dir,
        float speed, float delay=0f, bool spin=false, float rotation=0f) 
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }

        bul.GetComponent<StraightBulletBehavior>().speed = speed;
        bul.GetComponent<StraightBulletBehavior>().direction = dir;
        bul.GetComponent<StraightBulletBehavior>().spin = spin;
        bul.GetComponent<BulletBehavior>().indicate_time = delay;
        bul.transform.position = pos;
        bul.transform.rotation = Quaternion.Euler(0f,0f,rotation);
        bul.GetComponent<BulletBehavior>().Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected GameObject SpawnLaser(GameObject l, Vector2 start, Vector2 target, float width=.3f, 
        float delay=1f, float lifetime=1f, GameObject source=null, bool destroy_source=false) 
    {
        GameObject laser = Instantiate(l, start, Quaternion.identity);
        LaserBehavior l_script = laser.GetComponent<LaserBehavior>();
        laser.GetComponent<LineRenderer>().SetPositions(new Vector3[] {start, target});
        l_script.delay = delay;
        l_script.width = width;
        l_script.lifetime = lifetime;
        l_script.source = source;
        l_script.destroy_source = destroy_source;
        return laser;
    }
    
    protected GameObject SpawnActionBullet(GameObject b, Vector2 start, Vector2 dest,
        float speed, ActionBulletBehavior.AtDestination act, float delay=0f) 
    {
        GameObject bul = Instantiate(b, start, Quaternion.identity);
        ActionBulletBehavior bul_behavior = bul.GetComponent<ActionBulletBehavior>();
        bul_behavior.speed = speed;
        bul_behavior.destination = dest;
        bul_behavior.at_destination = act;
        bul_behavior.indicate_time = delay;
        return bul;
    }
    
    protected GameObject SpawnActionBullet(GameObject[] pool, Vector2 start, Vector2 dest,
        float speed, ActionBulletBehavior.AtDestination act, float rotation=0f, float delay=0f) 
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }

        ActionBulletBehavior bul_behavior = bul.GetComponent<ActionBulletBehavior>();
        bul.transform.position = start;
        bul.transform.rotation = Quaternion.Euler(0f,0f,rotation);
        bul_behavior.speed = speed;
        bul_behavior.destination = dest;
        bul_behavior.at_destination = act;
        bul_behavior.indicate_time = delay;
        bul.GetComponent<BulletBehavior>().Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected IEnumerator WaitForFixedDuration(float duration) {
        for (float d = duration; d > 0f; d -= Time.fixedDeltaTime) {
            yield return new WaitForFixedUpdate();
        }
    }
}
