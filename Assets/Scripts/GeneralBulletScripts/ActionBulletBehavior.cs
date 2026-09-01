using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBulletBehavior : BulletBehavior
{
    public Vector2 direction;
    public Vector2 destination;
    public delegate void AtDestination(GameObject self);
    public AtDestination at_destination;

    public delegate void AtInterval(GameObject self);
    private class Interval
    {
        public AtInterval at_interval;
        public float interval = 1f;
        public float intervalTimer = 0f;

        public Interval(AtInterval f, float t)
        {
            at_interval = f;
            interval = t;
        }
    }

    private List<Interval> at_interval;

    public override void Spawn()
    {
        base.Spawn();

        if (at_interval != null)
            at_interval.Clear();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        
        if (delay_time >= 0f)
        {
            delay_time -= Time.fixedDeltaTime;
            transform.localScale = new Vector3(Mathf.Lerp(final_scale_x, final_scale_y * 3, delay_time / indicate_time)
                                                , Mathf.Lerp(final_scale_y, final_scale_y * 3, delay_time / indicate_time),
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
            if (at_interval != null)
            {
                foreach (Interval interval in at_interval)
                {
                    interval.intervalTimer += Time.fixedDeltaTime;
                    if (interval.intervalTimer >= interval.interval)
                    {
                        interval.intervalTimer -= interval.interval;
                        if (at_interval != null)
                            interval.at_interval(gameObject);
                    }
                }
            }
            // Move towards destination, and if bullet just moved to destination, call at_destination
            if (at_destination != null)
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, destination, speed * Time.fixedDeltaTime);
            else
                transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);

            if (at_destination != null && !(((Vector2)transform.localPosition - destination).sqrMagnitude >= 0.001f))
            {
                at_destination(gameObject);
            }
        }
    }

    public void AddInterval(AtInterval f, float t)
    {
        if (at_interval == null)
            at_interval = new List<Interval>();

        Interval newInterval = new Interval(f, t);
        at_interval.Add(newInterval);
    }
}
