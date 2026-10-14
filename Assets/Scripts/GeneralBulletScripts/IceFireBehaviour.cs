using UnityEngine;

public class IceFireBehaviour : BulletBehavior
{
    [SerializeField] private Vector2 _targetPos;
    private Vector2 _startPos;
    private bool _movingToTarget = false;
    private float _moveElapsedTime = 0f;

    private EnemyBehavior _enemy;

    protected override void DelayAnimate(float t)
    {
        base.DelayAnimate(t);
        Debug.Log($"{t}: {transform.localScale.x} {transform.localScale.y}");
    }

    public override void Spawn()
    {
        base.Spawn();
        _enemy = GameObject.Find("Enemy").GetComponent<EnemyBehavior>();
        _targetPos = transform.position;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_delayTime > 0f) return;

        if (_movingToTarget)
        {
            _moveElapsedTime += Time.fixedDeltaTime;
            transform.position = Vector2.Lerp(_startPos, _targetPos, _enemy.InOutCubic(_moveElapsedTime/3f));
            if (_moveElapsedTime > 3f)
                _movingToTarget = false;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.6f, LayerMask.GetMask("EnemyBullets"));
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out CreepingFrostBehaviour cfb))
            {
                if (!cfb.Paused)
                {
                    cfb.Melt();
                }
            }
        }
        hits = Physics2D.OverlapCircleAll(transform.position, 1.2f, LayerMask.GetMask("EnemyBullets"));
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out CreepingFrostBehaviour cfb))
            {
                if (!cfb.Paused)
                {
                    cfb.Refresh();
                }
            }
        }
    }

    public void SetTargetPosition(Vector2 targetPos)
    {
        _startPos = transform.position;
        _targetPos = targetPos;
        _movingToTarget = true;
        _moveElapsedTime = 0f;
    }
}
