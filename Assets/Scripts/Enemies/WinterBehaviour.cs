using UnityEngine;
using System.Collections;

public class WinterBehaviour : EnemyBehavior
{
    private static WinterBehaviour Instance;
    private GameObject[] _frostPool;
    protected override void InitEnemy()
    {
        _frostPool = _bulletPools["Creeping Frost"];

        _patternList = new Pattern[]
        {
            new Pattern("Spell1", 5000, 90, true, Spell1_aux1())
        };
    }

    public IEnumerator Spell1_aux1()
    {
        yield return WaitForFixedDuration(1f);
        SpawnFrostBullet(_frostPool, transform.position, delay:0.3f);
        SpawnFrostBullet(_frostPool, transform.position + (Vector3)Random.onUnitCircle*5f, delay:0.3f);
    }

    // Spawning function for the unique Creeping Frost bullet type used by this enemy
    private GameObject SpawnFrostBullet(
        GameObject[] pool, Vector2 position, float delay=0f
    )
    {
        GameObject bul = FindNextBullet(pool);
        if (bul == null) { return null; }
        foreach (BulletBehavior bb in bul.GetComponents<BulletBehavior>())
            bb.enabled = bb is CreepingFrostBehaviour;        
        CreepingFrostBehaviour cbb = bul.GetComponent<CreepingFrostBehaviour>();

        bul.transform.position = position;
        cbb.indicateTime = delay;
        cbb.Spawn();
        bul.SetActive(true);
        return bul;
    }
    // Public static overload of the SpawnFrostBullet method to give access to this method to
    // external objects (i.e., the creeping frost bullets, so that they can spawn more of themselves)
    public static GameObject SpawnFrostBullet(Vector2 position, float delay=0f)
    {
        if (Instance == null)
            Instance = GameObject.Find("Enemy").GetComponent<WinterBehaviour>();
        
        if (Instance == null)
            return null;

        return Instance.SpawnFrostBullet(Instance._frostPool, position, delay);
    }
}
