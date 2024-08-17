using System.Collections;
using System.Collections.Generic;
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
    // Graze fields
    public float graze_val = 10f;
    public bool grazing = false;
    public float dist = -1f;
    public Color graze_color = new Color(1f, .75f, .75f, 1f);
    public Color default_color = new Color(1f, 1f, 1f, 1f);

    public virtual void Spawn() {
        final_scale_x = transform.localScale.x;
        final_scale_y = transform.localScale.y;
        delay_time = indicate_time;
    }

    public void Graze(Transform player) {
        grazing = true;
        StartCoroutine("Grazing", player);
    }

    IEnumerator Grazing(Transform player) {
        while (grazing) {
            if (graze_val > 0f) {
                dist = Vector2.Distance(player.position, transform.position);
                dist = Mathf.Clamp(dist, 0f, 1f);
                graze_val -= 1f - dist;
            }
            else { grazing = false; }
            yield return new WaitForFixedUpdate();
        }
    }

    public void ResetGraze() {
        graze_val = 10f;
        grazing = false;
    }

    /*void OnTriggerExit2D(Collider2D other) {
        if (!reserve && GetComponent<Collider2D>().enabled && other.gameObject.CompareTag("Boundary")) {
            DestroySelf();
        }
    }*/

    public void DestroySelf() {
        transform.localScale = new Vector3(final_scale_x, final_scale_y, 1f);
        gameObject.SetActive(false);
    }
}
