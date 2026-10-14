using UnityEngine;

public class IceFireBehaviour : BulletBehavior
{
    protected override void DelayAnimate(float t)
    {
        base.DelayAnimate(t);
        Debug.Log($"{t}: {transform.localScale.x} {transform.localScale.y}");
    }
}
