using System.Collections;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed;
    public float indicate_time;
    protected float delay_time;
    protected float final_scale_x;
    protected float final_scale_y;
    public bool spin = false;
    public bool reserve = false;

    [Header("Graze Properties")]
    [SerializeField] bool can_graze = true;
    public float graze_val = 10f;
    public bool grazing = false;
    public float dist = -1f;
    private SpriteRenderer rend;
    public Color graze_color = new Color(1f, .75f, .75f, 1f);
    public Color default_color = new Color(1f, 1f, 1f, 1f);

    void Awake()
    {
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!reserve && (Mathf.Abs(transform.position.x) > 20f || Mathf.Abs(transform.position.y) > 20f))
            DestroySelf();
    }

    public virtual void Spawn()
    {
        final_scale_x = transform.localScale.x;
        final_scale_y = transform.localScale.y;
        delay_time = indicate_time;
    }

    public void Graze(Transform player) {
        if (!can_graze) { return; }

        grazing = true;
        default_color = rend.color;
        rend.color = graze_color;
        StartCoroutine("Grazing", player);
    }

    IEnumerator Grazing(Transform player)
    {
        while (grazing)
        {
            if (graze_val > 0f)
            {
                dist = (transform.position - player.position).sqrMagnitude;
                dist = Mathf.Clamp(dist, 0f, 1f);
                graze_val -= 1f - dist;
                rend.transform.localPosition = Random.insideUnitCircle * 0.008f;
            }
            else
            {
                grazing = false;
            }
            yield return new WaitForFixedUpdate();
        }
        rend.transform.localPosition = Vector3.zero;
        rend.color = default_color;
    }

    public void ResetGraze() {
        rend.color = default_color;
        rend.transform.localPosition = Vector3.zero;
        graze_val = 10f;
        grazing = false;
    }

    public void DestroySelf() {
        ResetGraze();
        transform.localScale = new Vector3(final_scale_x, final_scale_y, 1f);
        gameObject.SetActive(false);
    }
}
