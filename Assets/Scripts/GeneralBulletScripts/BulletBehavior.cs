using System.Collections;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public const float RADTODEG = 180/Mathf.PI;

    private Transform _poolParent;

    public float indicateTime;
    protected float _delayTime;
    public bool IsSpawned { get => _delayTime <= 0f; }
    protected bool _paused = false;
    public bool Paused { get => _paused; set => _paused = value; }
    [SerializeField] protected float _finalScaleX;
    [SerializeField] protected float _finalScaleY;
    public int spin = 0;
    public bool reserve = false;
    protected Sprite _defaultSprite;
    protected int _baseSortingOrder;
    [SerializeField] Sprite delaySpawnVFXSprite;
    [SerializeField] bool dontDestroyOnPatternChange = false;

    [Header("Graze Fields")]
    [SerializeField] private bool _grazing = false;
    private float _grazeDistToPlayer = -1f;
    private float _grazeVal = 10f;
    private bool _canGraze = true;
    public float GrazeVal { get => _grazeVal; }
    public Color graze_color = new Color(1f, .75f, .75f, 1f);
    public Color default_color = new Color(1f, 1f, 1f, 1f);
    private Coroutine grazingCoroutine;

    protected SpriteRenderer rend;

    void Awake()
    {
        rend = GetComponentInChildren<SpriteRenderer>();
        if (rend == null) rend = gameObject.AddComponent<SpriteRenderer>();
        _defaultSprite = rend.sprite;
        default_color = rend.color;
        _baseSortingOrder = rend.sortingOrder;
        _finalScaleX = transform.localScale.x;
        _finalScaleY = transform.localScale.y;
    }

    protected virtual void FixedUpdate()
    {
        if (_paused) return;
        if (_delayTime > 0)
        {
            _delayTime -= Time.fixedDeltaTime;
            DelayAnimate(1f - _delayTime / indicateTime);

            if (_delayTime <= 0f)
            {
                rend.color = default_color;
                transform.localScale = new Vector3(_finalScaleX, _finalScaleY, 1f);
                rend.sprite = _defaultSprite;
                rend.sortingOrder = _baseSortingOrder;
            }
        }
    }

    protected virtual void DelayAnimate(float t)
    {
        transform.localScale = new Vector3(Mathf.Lerp(_finalScaleX * 2, _finalScaleX / 3, t),
                                            Mathf.Lerp(_finalScaleY * 2, _finalScaleY / 3, t),
                                            1f);
        rend.color = default_color * new Color(1f, 1f, 1f, Mathf.Lerp(0.5f, 0.8f, -1 - t*2));
    }

    public virtual void Spawn()
    {
        _delayTime = indicateTime;
        _paused = false;
        if (_delayTime > 0f)
        {
            rend.color = default_color * new Color(1f, 1f, 1f, 0.5f);
            rend.sprite = delaySpawnVFXSprite == null ? _defaultSprite : delaySpawnVFXSprite;
            DelayAnimate(0f);
            rend.sortingOrder = 100;
        }
        else
        {
            rend.color = default_color;
            rend.sortingOrder = _baseSortingOrder;
        }
    }

    public bool Graze(Transform player)
    {
        if (!_canGraze) { return false; }

        _grazing = true;
        rend.color = graze_color;
        grazingCoroutine = StartCoroutine("Grazing", player);
        return true;
    }

    IEnumerator Grazing(Transform player)
    {
        while (_grazing)
        {
            if (_grazeVal > 0f)
            {
                _grazeDistToPlayer = (transform.position - player.position).sqrMagnitude;
                _grazeDistToPlayer = Mathf.Clamp(_grazeDistToPlayer, 0f, 1f);
                _grazeVal -= 1f - _grazeDistToPlayer;
                rend.transform.localPosition = Random.insideUnitCircle * 0.008f;
            }
            else
            {
                _grazing = false;
                GameManager.Instance.ScoreGraze();
                OnGraze();
            }
            yield return new WaitForFixedUpdate();
        }
        rend.transform.localPosition = Vector3.zero;
        rend.color = default_color;
    }
    protected virtual void OnGraze() { }
    public void ResetGraze() {
        if (!_canGraze) { return; }
        if (rend == null) { return; }
        rend.color = default_color;
        rend.transform.localPosition = Vector3.zero;
        _grazeVal = 10f;
        _grazing = false;

        if (grazingCoroutine != null)
            StopCoroutine(grazingCoroutine);
    }

    public void DestroySelf() {
        if (dontDestroyOnPatternChange) return;
        ResetGraze();
        transform.localScale = new Vector3(_finalScaleX, _finalScaleY, 1f);
        gameObject.SetActive(false);
        if (_poolParent != null)
            transform.SetParent(_poolParent);
    }

    public void SetSprite(Sprite spr)
    {
        rend.sprite = spr;
        _defaultSprite = spr;
    }

    public void SetPoolParent(Transform parent)
    {
        _poolParent = parent;
        transform.SetParent(_poolParent);
    }
}
