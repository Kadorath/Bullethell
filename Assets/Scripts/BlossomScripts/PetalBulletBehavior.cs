using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalBulletBehavior : StraightBulletBehavior
{
    public Vector2 wind_force;
    [SerializeField] Sprite petalSpr;
    [SerializeField] Sprite rainSpr;
    [SerializeField] int angletoturn = 0;
    private enum State
    {
        Petal,
        Rain
    }
    private State state;

    void Start()
    {
        state = State.Petal;
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
                Debug.DrawRay(transform.position, transform.up, Color.red);
                Debug.DrawRay(transform.position, direction*speed+wind_force);
                angletoturn = (int)(Vector3.Angle(transform.up, direction*speed + wind_force) * ccw);
                transform.Rotate(
                    0f, 0f,
                    angletoturn
                );
                ApplyEffect();
                break;
        }

        float loopY = transform.position.y < -12f ? 12f : (transform.position.y > 12f ? -12f : transform.position.y);
        float loopX = transform.position.x < -12f ? 12f : (transform.position.x > 12f ? -12f : transform.position.x);
        transform.position = new Vector3(loopX, loopY, transform.position.z);
    }

    protected override void ApplyEffect()
    {
        transform.Translate(wind_force * Time.deltaTime, Space.World);

        if (state == State.Petal && wind_force.sqrMagnitude > 0f)
            wind_force *= 0.99f;
    }

    private void Transform(object sender, System.EventArgs e) {
        switch (state)
        {
            case State.Petal:
                rend.sprite = rainSpr;
                speed = 3f;
                state = State.Rain;
                break;
            case State.Rain:
                rend.sprite = petalSpr;
                transform.localRotation = Quaternion.Euler(0f, 0f, 90f + Random.Range(-10f, 10f));
                wind_force = Vector2.zero;
                speed = 0.25f;
                state = State.Petal;
                break;
        }
    }
}
