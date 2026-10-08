using UnityEngine;

public class GravityBulletBehaviour : StraightBulletBehavior
{
    public float gravityScale = 1f;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        direction += Vector2.down*gravityScale*Time.fixedDeltaTime;
        // direction = new Vector2(direction.x, Mathf.Min(1f, direction.y));
    }
}