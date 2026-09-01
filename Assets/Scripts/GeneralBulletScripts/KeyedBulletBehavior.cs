using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyedBulletBehavior : StraightBulletBehavior
{
    private Dictionary<int, float> rotationKeyframes;
    [SerializeField] int lastRotationKey = 0;
    [SerializeField] int nextRotationKey = 0;
    [SerializeField] float elapsedTime = 0f;
    [SerializeField] int elapsedTicks = 0;
    private Dictionary<int, float> speedKeyframes;
    [SerializeField] int lastSpeedKey = 0;
    [SerializeField] int nextSpeedKey = 0;


    public override void Spawn()
    {
        base.Spawn();
        elapsedTime = 0f;
        elapsedTicks = 0;
        lastRotationKey = 0;
        lastSpeedKey = 0;
    }

    public void SetRotationKeyframes(Dictionary<int, float> keyframes)
    {
        rotationKeyframes = new Dictionary<int, float>(keyframes);
        if (!rotationKeyframes.ContainsKey(0))
            rotationKeyframes[0] = transform.rotation.eulerAngles.z;
        
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            rotationKeyframes[0]
        );

        nextRotationKey = FindNextKey(rotationKeyframes, 0);
    }

    public void SetSpeedKeyframes(Dictionary<int, float> keyframes)
    {
        speedKeyframes = new Dictionary<int, float>(keyframes);
        if (!speedKeyframes.ContainsKey(0))
            speedKeyframes[0] = speed;
        speed = speedKeyframes[0];
        nextSpeedKey = FindNextKey(speedKeyframes, 0);
    }

    protected override void FixedUpdate()
    {
        direction = transform.up;

        base.FixedUpdate();

        elapsedTime += Time.fixedDeltaTime;
        elapsedTicks += 1;

        // Rotate by interpolating between keyframes
        if (nextRotationKey != -1 && rotationKeyframes != null)
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                Mathf.Lerp(rotationKeyframes[lastRotationKey], rotationKeyframes[nextRotationKey], ((float)elapsedTicks-lastRotationKey) / (nextRotationKey-lastRotationKey))
            );

            if (elapsedTicks >= nextRotationKey)
            {
                lastRotationKey = nextRotationKey;
                nextRotationKey = FindNextKey(rotationKeyframes, lastRotationKey);
            }
        }

        // Modify speed by interpolating between keyframes
        if (nextSpeedKey != -1 && speedKeyframes != null)
        {
            speed = Mathf.Lerp(speedKeyframes[lastSpeedKey], speedKeyframes[nextSpeedKey], ((float)elapsedTicks-lastSpeedKey) / (nextSpeedKey-lastSpeedKey));

            if (elapsedTicks >= nextSpeedKey)
            {
                lastSpeedKey = nextSpeedKey;
                nextSpeedKey = FindNextKey(speedKeyframes, lastSpeedKey);
            }
        }
    }

    private int FindNextKey(Dictionary<int, float> dict, int start)
    {
        List<int> keys = dict.Keys.ToList();

        foreach(int key in keys)
        {
            if (key > start)
                return key;
        }

        return -1;
    }
}
