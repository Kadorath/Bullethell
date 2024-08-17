using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapshotBehavior : MonoBehaviour
{
    public Collider2D[] captured_frame;
    public Vector2[] captured_pos;
    public Vector3[] frame_points;
    private float x1;
    private float x2;
    private float y1;
    private float y2;
    public LineRenderer frame;

    public SpriteRenderer flash;
    public Camera my_camera;
    public RenderTexture my_tex;
    public GameObject image_frame;

    public bool lock_player = false;
    public GameObject player;
    public Vector2 captured_player_pos;

    private GameObject gameManager;

    void Start() {
        gameManager = GameObject.Find("GameManager");
        player = GameObject.Find("Player");

        frame = GetComponent<LineRenderer>();
        frame_points = new Vector3[4];

        flash = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        my_camera = transform.GetChild(1).gameObject.GetComponent<Camera>();
        my_tex = new RenderTexture(256,256,16,RenderTextureFormat.ARGB32);
        my_camera.targetTexture = my_tex;
        // This feels like a bandaid solution. Feels like it shouldn't be necessary
        my_tex.Release();
        image_frame.GetComponent<RawImage>().texture = my_tex;
    }

    void FixedUpdate() {
        if (flash.color.a >= 0f) {
            flash.color = new Color(1f,1f,1f,flash.color.a-0.1f);
        }

        if (lock_player) {
            float padding = 0.25f;
            x1 = transform.position.x-transform.localScale.x + padding;
            y1 = transform.position.y-transform.localScale.y + padding;
            x2 = transform.position.x+transform.localScale.x - padding;
            y2 = transform.position.y+transform.localScale.y - padding;
            player.transform.position = 
                new Vector3(Mathf.Clamp(player.transform.position.x,x1,x2),
                            Mathf.Clamp(player.transform.position.y,y1,y2),
                            0f);
        }
    }

    public void Indicate() {
        flash.color = new Color(1f,1f,1f,1f);
    }

    public Collider2D[] Snapshot() {
        my_camera.Render();
        frame.GetPositions(frame_points);
        float padding = 0.2f+frame.widthMultiplier;
        captured_frame = Physics2D.OverlapAreaAll(
            new Vector2(transform.localScale.x*frame_points[0].x + transform.position.x + padding,
                transform.localScale.y*frame_points[0].y + transform.position.y - padding),
            new Vector2(transform.localScale.x*frame_points[2].x + transform.position.x - padding,
                transform.localScale.y*frame_points[2].y + transform.position.y + padding),
            LayerMask.GetMask("EnemyBullets")
        );
        foreach (Collider2D bul in captured_frame) {
            bul.gameObject.GetComponent<BulletBehavior>().reserve = true;
        }
        if (lock_player) {
            captured_pos = new Vector2[captured_frame.Length];
            for (int i = 0; i < captured_frame.Length; i ++) {
                captured_pos[i] = captured_frame[i].transform.position;
            }
            captured_player_pos = player.transform.position;
        }
        flash.color = new Color(1f,1f,1f,1f);
        return captured_frame;
    }

    public void DevelopCapture() {
        float padding = 0.2f+frame.widthMultiplier;
        Collider2D[] bul_to_remove = Physics2D.OverlapAreaAll(
            new Vector2(transform.localScale.x*frame_points[0].x + transform.position.x + padding,
                transform.localScale.y*frame_points[0].y + transform.position.y - padding),
            new Vector2(transform.localScale.x*frame_points[2].x + transform.position.x - padding,
                transform.localScale.y*frame_points[2].y + transform.position.y + padding),
            LayerMask.GetMask("EnemyBullets")
        );
        foreach (Collider2D bul in bul_to_remove) {
            bul.gameObject.SetActive(false);
        }

        for (int i = 0; i < captured_frame.Length; i ++) {
            captured_frame[i].gameObject.transform.position = captured_pos[i];
            captured_frame[i].gameObject.GetComponent<BulletBehavior>().reserve = false;
            captured_frame[i].gameObject.SetActive(true);
        }
        player.transform.position = captured_player_pos;

        my_camera.targetTexture.Release();
        flash.color = new Color(1f,1f,1f,1f);
    }

    public void FreezeFrame() {
        foreach (Collider2D bul in captured_frame) {
            bul.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Background");
            bul.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            bul.gameObject.GetComponent<BulletBehavior>().speed = 0f;
            bul.gameObject.GetComponent<BulletBehavior>().default_color = new Color(188f/255f, 188f/255f, 188f/255f, 1f);
            bul.gameObject.GetComponent<BulletBehavior>().ResetGraze();
            bul.gameObject.transform.SetParent(transform, true);
            bul.enabled = false;
        }
    }

    public void UnfreezeFrame() {
        foreach (Collider2D bul in captured_frame) {
            bul.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Bullets");
            StraightBulletBehavior bul_script = bul.gameObject.GetComponent<StraightBulletBehavior>();
            bul_script.default_color = new Color(1f, 1f, 1f, 1f);    
            bul_script.direction = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * bul_script.direction;
            bul_script.speed = 2f;
            bul_script.reserve = false;
            bul.gameObject.transform.SetParent(gameManager.gameObject.transform, true);
            bul.enabled = true;
        }

        my_camera.targetTexture.Release();
    }
}
