using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHEntity : MonoBehaviour
{
    public const float RADTODEG = 180/Mathf.PI;
    public const float DEGTORAD = Mathf.PI/180;
    protected float BOUND_X { get { return GameManager.BOUND_X; } set { GameManager.BOUND_X = value; }}
    protected float BOUND_Y { get { return GameManager.BOUND_Y; } set { GameManager.BOUND_Y = value; }}

    protected static GameObject FindNextBullet(GameObject[] pool) {
        for (int i = 0; i < pool.Length; i ++) {
            if (!pool[i].activeSelf) {
                foreach (BulletBehavior bb in pool[i].GetComponents<BulletBehavior>())
                    bb.ResetGraze();   
                return pool[i];
            }
        }
        return null;
    }

    public void Update()
    {
        Debug.DrawLine(new Vector3(-BOUND_X, -BOUND_Y, 0f), new Vector3(BOUND_X, -BOUND_Y, 0f), Color.red);
        Debug.DrawLine(new Vector3(-BOUND_X, BOUND_Y, 0f), new Vector3(BOUND_X, BOUND_Y, 0f), Color.red);
        Debug.DrawLine(new Vector3(-BOUND_X, -BOUND_Y, 0f), new Vector3(-BOUND_X, BOUND_Y, 0f), Color.red);
        Debug.DrawLine(new Vector3(BOUND_X, -BOUND_Y, 0f),  new Vector3(BOUND_X, BOUND_Y, 0f), Color.red);
    }

    /// <summary>
    /// Spawns a straight bullet with the given parameters. Only the first three parameters are required.
    /// </summary>
    /// <param name="pool">The object pool from which to spawn a bullet.</param>
    /// <param name="origin">The starting position of the bullet, in world space.</param>
    /// <param name="direction">The starting direction of the bullet, in world space.</param>
    /// <param name="speed">The starting speed of the bullet.</param>
    /// <param name="delay">The time in seconds after spawning that the bullet takes to "appear" (the bullet's sprite begins enlarged and scales down to its base size over this time).</param>
    /// <param name="rotation">The beginning rotation of the sprite of the bullet.</param>
    /// <param name="alignWithDir">If true, the bullet object rotates to align its forward vector with its current direction.</param>
    /// <param name="speedChangeDuration">Time duration starting from the end of delay over which the bullet's speed mod goes from startSpeedMod to finalSpeedMod. If 0, speed mod does not change.</param>
    /// <param name="startSpeedMod">Starting speed mod for the bullet.</param>
    /// <param name="finalSpeedMod">Final speed mod for the bullet. This will be the bullet's speed modifier once it has been active for speedChangeDuration seconds.</param>
    /// <returns></returns>
    protected GameObject SpawnStraightBullet(
        GameObject[] pool, Vector2 origin, Vector2 direction,
        float speed=0f, float delay=0f, float rotation=0f, float turnSpeed=0f, bool alignWithDir=false, 
        float speedChangeDuration=0f, float startSpeedMod=0f, float finalSpeedMod=0f
    )
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb.GetType() == typeof(StraightBulletBehavior);      
        StraightBulletBehavior sbb = bul.GetComponent<StraightBulletBehavior>();

        bul.transform.position = origin;
        sbb.direction = direction;
        sbb.speed = speed;
        sbb.indicateTime = delay;
        bul.transform.rotation = Quaternion.Euler(0f,0f,rotation);
        sbb.turnSpeed = turnSpeed;
        sbb.alignWithDirection = alignWithDir;
        sbb.speedChangeDuration = speedChangeDuration;
        sbb.startSpeedMod = startSpeedMod;
        sbb.finalSpeedMod = finalSpeedMod;
        sbb.Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected GameObject SpawnStraightBullet(
        GameObject obj, Vector2 origin, Vector2 direction,
        float speed=0f, float delay=0f, float rotation=0f, float turnSpeed=0f, bool alignWithDir=false, 
        float speedChangeDuration=0f, float startSpeedMod=0f, float finalSpeedMod=0f
    )
    {
        GameObject bul = Instantiate(obj);
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb is StraightBulletBehavior;

        StraightBulletBehavior sbb = bul.GetComponent<StraightBulletBehavior>();
        bul.transform.position = origin;
        sbb.direction = direction;
        sbb.speed = speed;
        sbb.indicateTime = delay;
        bul.transform.rotation = Quaternion.Euler(0f,0f,rotation);
        sbb.turnSpeed = turnSpeed;
        sbb.alignWithDirection = alignWithDir;
        sbb.speedChangeDuration = speedChangeDuration;
        sbb.startSpeedMod = startSpeedMod;
        sbb.finalSpeedMod = finalSpeedMod;
        sbb.Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected GameObject SpawnGravityBulet(GameObject[] pool, Vector2 origin, Vector2 direction,
        float speed=0f, float delay=0f, float rotation=0f, float turnSpeed=0f, bool alignWithDir=false, 
        float speedChangeDuration=0f, float startSpeedMod=0f, float finalSpeedMod=0f, float gravityScale=1f)
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb.GetType() == typeof(GravityBulletBehaviour);      
        GravityBulletBehaviour gbb = bul.GetComponent<GravityBulletBehaviour>();
        bul.transform.position = origin;
        gbb.direction = direction;
        gbb.speed = speed;
        gbb.indicateTime = delay;
        bul.transform.rotation = Quaternion.Euler(0f,0f,rotation);
        gbb.turnSpeed = turnSpeed;
        gbb.alignWithDirection = alignWithDir;
        gbb.speedChangeDuration = speedChangeDuration;
        gbb.startSpeedMod = startSpeedMod;
        gbb.finalSpeedMod = finalSpeedMod;
        gbb.gravityScale = gravityScale;
        gbb.Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected GameObject SpawnKeyedBullet(GameObject[] pool, Vector2 pos, Dictionary<int, float> rotationKF, Dictionary<int, float> speedKF, float delay=0f)
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb is KeyedBulletBehavior;

        KeyedBulletBehavior kbb = bul.GetComponent<KeyedBulletBehavior>();
        kbb.indicateTime = delay;
        bul.transform.position = pos;
        kbb.Spawn();
        if (rotationKF != null)
            kbb.SetRotationKeyframes(rotationKF);
        if (speedKF != null)
            kbb.SetSpeedKeyframes(speedKF);
        bul.SetActive(true);
        return bul;
    }

    protected GameObject SpawnLaser(GameObject l, Vector2 start, Vector2 target, float width = .3f,
        float delay = 1f, float lifetime = 1f, GameObject source = null, bool destroy_source = false)
    {
        GameObject laser = Instantiate(l, start, Quaternion.identity);
        LaserBehavior l_script = laser.GetComponent<LaserBehavior>();
        laser.GetComponent<LineRenderer>().SetPositions(new Vector3[] { start, target });
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
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb is ActionBulletBehavior;

        ActionBulletBehavior bul_behavior = bul.GetComponent<ActionBulletBehavior>();
        bul_behavior.speed = speed;
        bul_behavior.destination = dest;
        bul_behavior.at_destination = act;
        bul_behavior.indicateTime = delay;
        bul.GetComponent<BulletBehavior>().Spawn();        
        return bul;
    }
    
    /// <summary>
    /// Spawns an action bullet, with a target destination and a delegate to perform on arrival.
    /// </summary>
    /// <param name="pool">GameObject pool of bullet objects to find an available bullet object from</param>
    /// <param name="start">Initial Vector2 position to spawn bullet at</param>
    /// <param name="dest">Vector2 position that bullet move's towards</param>
    /// <param name="speed">Speed at which bullet moves towards destination</param>
    /// <param name="act">Delegate that is executed once bullet has arrived at destination</param>
    /// <param name="rotation">Initial rotation for the bullet to be set at (indepedent from moving direction)</param>
    /// <param name="delay">Spawning time before bullet becomes active</param>
    /// <returns></returns>
    protected GameObject SpawnActionBullet(GameObject[] pool, Vector2 start, Vector2 dest,
        float speed, ActionBulletBehavior.AtDestination act, float rotation=0f, float delay=0f) 
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb is ActionBulletBehavior;

        ActionBulletBehavior bul_behavior = bul.GetComponent<ActionBulletBehavior>();
        bul.transform.position = start;
        bul.transform.rotation = Quaternion.Euler(0f,0f,rotation);
        bul_behavior.speed = speed;
        bul_behavior.destination = dest;
        bul_behavior.at_destination = act;
        bul_behavior.indicateTime = delay;
        bul.GetComponent<BulletBehavior>().Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected GameObject SpawnActionBullet(GameObject[] pool, Vector2 start, Vector2 dir, float speed, float rotation=0f, float delay=0f)
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb is ActionBulletBehavior;
    
        ActionBulletBehavior abb = bul.GetComponent<ActionBulletBehavior>();
        bul.transform.position = start;
        bul.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        abb.speed = speed;
        abb.direction = dir;
        abb.indicateTime = delay;
        abb.Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected GameObject SpawnFollowerBullet(GameObject[] pool, Transform followTarget, int followDist)
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { Debug.LogWarning("No free follower bullet found"); return null; }
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb is FollowerBulletBehavior;

        FollowerBulletBehavior fbb = bul.GetComponent<FollowerBulletBehavior>();
        bul.transform.position = followTarget.transform.position;
        bul.transform.rotation = followTarget.transform.rotation;
        fbb.followTarget = followTarget;
        fbb.followDistance = followDist;
        fbb.indicateTime = 0f;
        fbb.Spawn();
        bul.SetActive(true);
        return bul;
    }

    protected IEnumerator WaitForFixedDuration(float duration) {
        for (float d = duration; d > 0f; d -= Time.fixedDeltaTime) {
            yield return new WaitForFixedUpdate();
        }
    }

    // Easing Functions
    public float InOutQuint(float x) => x < 0.5f ? 16f * x * x * x * x * x : 1 - Mathf.Pow(-2f * x + 2, 5) / 2f;
    public float InOutCubic(float x) => x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2 * x + 2, 3) / 2f;
    public float InOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5f
        ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1f) * 2f * x - c2)) / 2f
        : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
    }
}
