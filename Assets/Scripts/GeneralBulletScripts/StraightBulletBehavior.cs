using UnityEngine;

public class StraightBulletBehavior : BulletBehavior
{
    public Vector2 direction;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (delay_time >= 0f)
        {
            delay_time -= Time.fixedDeltaTime;
            transform.localScale = new Vector3(Mathf.Lerp(final_scale_x / 3, final_scale_y * 2, delay_time / indicate_time)
                                                , Mathf.Lerp(final_scale_y / 3, final_scale_y * 2, delay_time / indicate_time),
                                                1f);
            rend.color = default_color * new Color(1f, 1f, 1f, Mathf.Lerp(0.8f, 0.5f, delay_time / indicate_time / 2));

            if (delay_time <= 0f)
            {
                rend.color = default_color;
                transform.localScale = new Vector3(final_scale_x, final_scale_y, 1f);
                rend.sprite = default_sprite;
                rend.sortingOrder -= 10;
            }
        }
        else
        {
            transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
            if (spin)
            {
                transform.Rotate(new Vector3(0f, 0f, 3f));
            }
        }
    }

    protected virtual void ApplyEffect() {}
}
