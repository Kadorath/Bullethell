using UnityEngine;

public class WindGustBehavior : StraightBulletBehavior
{
    [Header("Wind Config")]
    [SerializeField] float force;
    private Vector3 wind_dir;

    void Start()
    {
        wind_dir = transform.up * force;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        PetalBulletBehaviour petal = other.GetComponent<PetalBulletBehaviour>();

        if (petal != null)
        {
            if (petal.wind_force.sqrMagnitude < wind_dir.sqrMagnitude)
                petal.wind_force += (Vector2)transform.up * 0.2f * force;
        }
    }

    public void SetForce(float f) { force = f; }
}
