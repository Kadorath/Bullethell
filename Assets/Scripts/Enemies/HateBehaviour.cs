using UnityEngine;
using System.Collections;

public class HateBehaviour : EnemyBehavior
{
    private GameObject[] _defaultPool;
    protected override void InitEnemy()
    {
        _defaultPool = _bulletPools["Default"];

        _patternList = new Pattern[]
        {
            new Pattern("Spell1", 5000, 90, Spell1_aux1())
        };
    }

    public IEnumerator Spell1_aux1()
    {
        SpawnCircleRing(_defaultPool,transform.position, 16, speed:5f);
        yield return null;
    }
}
