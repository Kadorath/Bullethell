using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : BHEntity
{
    InputAction moveAction;
    InputAction fireAction;
    InputAction slowAction;

    [Header("Player Config")]
    [SerializeField] private float move_speed_fast = 7.5f;
    [SerializeField] private float move_speed_slow = 2f;
    private float move_speed;
    [SerializeField] private int fire_rate = 1;
    [SerializeField] private int fire_ct = 0;

    public GameObject bul_player;
    private GameObject[] my_pool;
    [SerializeField] private ParticleSystem death_ps;
    [SerializeField] private GameObject[] hardpoints;

    void Start()
    {
        // Set up input action references
        moveAction = InputSystem.actions.FindAction("Move");
        fireAction = InputSystem.actions.FindAction("Fire");
        slowAction = InputSystem.actions.FindAction("Slow");

        move_speed = slowAction.ReadValue<float>() > 0.5f ? move_speed_slow : move_speed_fast;

        my_pool = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>().CreatePool(bul_player, 150);

        hardpoints = new GameObject[4];
        hardpoints[0] = GameObject.Find("hardpoint1");
        hardpoints[1] = GameObject.Find("hardpoint2");
        hardpoints[2] = GameObject.Find("hardpoint3");
        hardpoints[3] = GameObject.Find("hardpoint4");

        death_ps = GameObject.Find("Death Particle System").GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {
        // player movement
        // Read in the current input values
        Vector2 move_in = moveAction.ReadValue<Vector2>();
        float h_in = move_in.x;
        float v_in = move_in.y;

        // Translate the player body
        transform.Translate(new Vector3(h_in, v_in, 0f) * move_speed * Time.fixedDeltaTime);

        // Clamp the player position in the screen bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -BOUND_X, BOUND_X), 
            Mathf.Clamp(transform.position.y, -BOUND_Y, BOUND_Y),
            transform.position.z);

        // player firing
        if (Input.GetAxis("Fire") > 0.05f) {
            if (fire_ct == 0) { 
                for (int i = 0; i < hardpoints.Length; i ++) {
                    SpawnStraightBullet(my_pool, 
                        hardpoints[i].transform.position,
                        Vector2.up, 22f);
                }
            }
            fire_ct ++;
            if (fire_ct == fire_rate) { fire_ct = 0; }
        }
    }

    void OnSlow(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
        {
            move_speed = move_speed_slow;
            hardpoints[0].transform.localPosition = new Vector3(-.75f, 1.25f, 0f);
            hardpoints[1].transform.localPosition = new Vector3(.75f, 1.25f, 0f);
            hardpoints[2].transform.localPosition = new Vector3(-.25f, 1.75f, 0f);
            hardpoints[3].transform.localPosition = new Vector3(.25f, 1.75f, 0f);
        }
        else
        {
            move_speed = move_speed_fast;
            hardpoints[0].transform.localPosition = new Vector3(-1.5f, .5f, 0f);
            hardpoints[1].transform.localPosition = new Vector3(1.5f, .5f, 0f);
            hardpoints[2].transform.localPosition = new Vector3(-.5f, 1.5f, 0f);
            hardpoints[3].transform.localPosition = new Vector3(.5f, 1.5f, 0f);
        }
    }

    public void PlayerDie() {
        death_ps.Emit(50);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Bullet")) {
            if (other.gameObject.GetComponent<BulletBehavior>().graze_val > 0f) {
                other.gameObject.GetComponent<BulletBehavior>().Graze(gameObject.transform);
            }
        }
    } 

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Bullet")) {
            if (other.gameObject.GetComponent<BulletBehavior>().graze_val > 0f) {
                other.gameObject.GetComponent<BulletBehavior>().ResetGraze();
            }
        }
    }
}
