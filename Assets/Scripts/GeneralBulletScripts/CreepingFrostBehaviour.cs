using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreepingFrostBehaviour : BulletBehavior
{
    private float _spreadTimer = 0f;
    private bool _hasSpread = false;

    public override void Spawn()
    {
        base.Spawn();
        _spreadTimer = 0.1f;
        _hasSpread = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _spreadTimer -= Time.fixedDeltaTime;
        if (!_hasSpread && _spreadTimer <= 0f)
        {
            _hasSpread = true;
            // _spreadTimer = Random.Range(4f, 8f);
            Spread();
        }

        if (_delayTime > 0) return;
    }

    protected override void DelayAnimate(float t)
    {
        transform.localScale = new Vector3(Mathf.Lerp(_finalScaleX / 3, _finalScaleX, t),
                                            Mathf.Lerp(_finalScaleY / 3, _finalScaleY, t),
                                            1f);
        rend.color = default_color * new Color(1f, 1f, 1f, Mathf.Lerp(0.1f, 0.8f, -1 + t*2));
    }

    private void Spread()
    {
        // For hexagon: 0.4f*Mathf.Cos(Mathf.PI/3),0.4f*Mathf.Sin(Mathf.PI/3)
        Vector3[] positions = new Vector3[]
        {
            transform.position+Vector3.right*0.4f,
            transform.position+new Vector3(0.2f,0.34641f,0f),
            transform.position+new Vector3(-0.2f,0.34641f,0f),
            transform.position+Vector3.left*0.4f,
            transform.position+new Vector3(-0.2f,-0.34641f,0f),
            transform.position+new Vector3(0.2f,-0.34641f,0f),
        };
        // {
        //     transform.position+Vector3.down*0.4f,
        //     transform.position+Vector3.up*0.4f,
        //     transform.position+Vector3.right*0.4f,
        //     transform.position+Vector3.left*0.4f
        // };
        foreach (Vector3 pos in positions)
        {
            if (!Physics2D.OverlapCircle(pos, 0.2f, LayerMask.GetMask("EnemyBullets","Player")))
            {
                if (Random.value < 0.5f)
                    WinterBehaviour.SpawnFrostBullet(pos, 0.1f);
            }
        }
    }


    protected override void OnGraze()
    {
        // DestroySelf();
    }
}
