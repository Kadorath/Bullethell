using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowerBulletBehavior : BulletBehavior
{
    public Transform followTarget;
    public int followDistance;
    [SerializeField] private Vector2[] recordedPositions;
    private int positionTracker = 0;
    private bool bufferFilled = false;

    public override void Spawn()
    {
        base.Spawn();

        recordedPositions = new Vector2[followDistance];
        positionTracker = 0;
        bufferFilled = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (followTarget != null)
        {
            if (bufferFilled) 
            {
                Vector2 oldPosition = transform.position;
                transform.position = recordedPositions[positionTracker];
                Vector2 direction = (Vector2)transform.position - oldPosition;
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
            }

            recordedPositions[positionTracker] = followTarget.position;

            positionTracker += 1;
            if (positionTracker >= followDistance)
            {
                bufferFilled = true;
                positionTracker = 0;
            }   
        }
    }
}
