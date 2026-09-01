using System.Collections;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public const float RADTODEG = 180/Mathf.PI;

    public float speed;
    public float indicate_time;
    protected float delay_time;
    [SerializeField] protected float final_scale_x;
    [SerializeField] protected float final_scale_y;
    public bool spin = false;
    public bool reserve = false;
    protected Sprite default_sprite;
    [SerializeField] Sprite delaySpawnVFXSprite;
    [SerializeField] bool dontDestroyOnPatternChange = false;

    [Header("Graze Properties")]
    [SerializeField] protected bool can_graze = true;
    public float graze_val = 10f;
    public bool grazing = false;
    public float dist = -1f;
    protected SpriteRenderer rend;
    public Color graze_color = new Color(1f, .75f, .75f, 1f);
    public Color default_color = new Color(1f, 1f, 1f, 1f);
    private Coroutine grazingCoroutine;

    void Awake()
    {
        rend = GetComponentInChildren<SpriteRenderer>();
        default_sprite = rend.sprite;
        default_color = rend.color;
        final_scale_x = transform.localScale.x;
        final_scale_y = transform.localScale.y;
    }

    protected virtual void FixedUpdate()
    {
        // if (!reserve && (Mathf.Abs(transform.position.x) > 20f || Mathf.Abs(transform.position.y) > 20f))
        //     DestroySelf();
    }

    public virtual void Spawn()
    {
        delay_time = indicate_time;

        if (delay_time > 0f)
        {
            rend.color = default_color * new Color(1f, 1f, 1f, 0.5f);
            rend.sprite = delaySpawnVFXSprite == null ? default_sprite : delaySpawnVFXSprite;
            rend.sortingOrder += 10;
        }
        else
            rend.color = default_color;
    }

    public bool IsSpawned() { return delay_time <= 0f; }

    public bool Graze(Transform player)
    {
        if (!can_graze) { return false; }

        grazing = true;
        rend.color = graze_color;
        grazingCoroutine = StartCoroutine("Grazing", player);
        return true;
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
        if (!can_graze) { return; }
        rend.color = default_color;
        rend.transform.localPosition = Vector3.zero;
        graze_val = 10f;
        grazing = false;

        if (grazingCoroutine != null)
            StopCoroutine(grazingCoroutine);
    }

    public void DestroySelf() {
        if (dontDestroyOnPatternChange) return;
        ResetGraze();
        transform.localScale = new Vector3(final_scale_x, final_scale_y, 1f);
        gameObject.SetActive(false);
    }
}
