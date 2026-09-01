using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalBulletBehavior : StraightBulletBehavior
{
    BlossomBehavior blossom;

    public Vector2 wind_force;
    [SerializeField] Sprite petalSpr;
    [SerializeField] Sprite rainSpr;
    private enum State
    {
        Petal,
        Rain
    }
    private State state;

    void Start()
    {
        state = State.Petal;
        blossom = GameObject.Find("Enemy").GetComponent<BlossomBehavior>();
        BlossomBehavior.Thunderclap += Transform;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        switch (state)
        {
            case State.Petal:
                ApplyEffect();
                break;
            case State.Rain:
                float ccw = -Mathf.Sign(Vector2.Dot(direction, (Vector2)transform.right));
                Vector2 motionVec = direction*speed+wind_force;
                Debug.DrawRay(transform.position, transform.up, Color.red);
                Debug.DrawRay(transform.position, motionVec * 0.5f);
                // angletoturn = (int)(Vector3.Angle(transform.up, direction*speed + wind_force) * ccw);
                // transform.Rotate(
                //     0f, 0f,
                //     angletoturn
                // );
                transform.rotation = Quaternion.Euler(0f, 0f, RADTODEG*Mathf.Atan2(motionVec.y, motionVec.x) - 90f);
                ApplyEffect();
                break;
        }

        float loopY = transform.position.y < -12f ? transform.position.y + 24f : (transform.position.y > 12f ? transform.position.y - 24f : transform.position.y);
        float loopX = transform.position.x < -9.6f ? transform.position.x + 19.2f : (transform.position.x > 9.6f ? transform.position.x - 19.2f : transform.position.x);
        transform.position = new Vector3(loopX, loopY, transform.position.z);
    }

    protected override void ApplyEffect()
    {
        transform.Translate(wind_force * Time.fixedDeltaTime, Space.World);

        if (state == State.Petal && wind_force.sqrMagnitude > 0f)
            wind_force *= 0.99f;
        else
            wind_force = Vector2.Lerp(wind_force, blossom.ambientWindDir, 0.8f);
    }

    private void Transform(object sender, System.EventArgs e) {
        can_graze = true;
        switch (state)
        {
            case State.Petal:
                default_sprite = rainSpr;
                speed = 2.5f;
                state = State.Rain;
                Spawn();
                break;
            case State.Rain:
                default_sprite = petalSpr;
                wind_force = transform.up;
                transform.localRotation = Quaternion.Euler(0f, 0f, 90f + Random.Range(-10f, 10f));
                speed = 0.25f;
                state = State.Petal;
                Spawn();
                break;
        }
    }

    void OnDestroy()
    {
        BlossomBehavior.Thunderclap -= Transform;
    }
}
