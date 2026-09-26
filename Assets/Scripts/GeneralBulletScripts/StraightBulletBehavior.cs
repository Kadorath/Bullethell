using System;
using UnityEngine;

public class StraightBulletBehavior : BulletBehavior
{
    [Header("Movement Fields")]
    public float speed;
    public Vector2 direction;
    public bool alignWithDirection;
    public float turnSpeed = 0f;

    // Some simple parameters to change speed after initial spawn, in order to
    // simulate momentum, without necessarily creating a KeyedBullet
    [Header("Speed Change Fields")]
    public float startSpeedMod;
    public float finalSpeedMod;
    public float speedChangeDuration;
    [SerializeField] private float _speedMod = 1f;
    public float SpeedMod { get => _speedMod; set => _speedMod = value; }
    private float _elapsedTime = 0f;
    public override void Spawn()
    {
        base.Spawn();
        _speedMod = 1f;
        _elapsedTime = 0f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _elapsedTime += Time.fixedDeltaTime;

        if (_delayTime > 0f)
        {
            return;
        }
        else
        {
            Vector2 moveDir = (direction * (speed*_speedMod));
            transform.Translate(moveDir * Time.fixedDeltaTime, Space.World);

            if (Mathf.Abs(speedChangeDuration) > 0.00001f && _elapsedTime <= speedChangeDuration)
            {
                _speedMod = Mathf.Lerp(startSpeedMod, finalSpeedMod, _elapsedTime / speedChangeDuration);
            }

            if (Mathf.Abs(turnSpeed) > 0.00001f)
            {
                float radsToTurn = Mathf.Deg2Rad*turnSpeed*Time.fixedDeltaTime;
                float turnSin = Mathf.Sin(radsToTurn);
                float turnCos = Mathf.Cos(radsToTurn);
                float newX = direction.x * turnCos - direction.y * turnSin;
                float newY = direction.x * turnSin + direction.y * turnCos;
                direction = new Vector2(newX, newY);
            }
            if (alignWithDirection)
                transform.rotation = Quaternion.Euler(0f, 0f, RADTODEG*Mathf.Atan2(moveDir.y, moveDir.x) - 90f);
            if (spin)
                rend.transform.Rotate(new Vector3(0f, 0f, 1f));
        }
    }

    protected virtual void ApplyEffect() {}
}
