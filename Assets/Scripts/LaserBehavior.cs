using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    private const float FULL_LENGTH = 20f;

    public float delay;
    public float width;
    public float lifetime;

    public GameObject source;
    public bool destroy_source;
    
    private LineRenderer line_renderer;
    private PolygonCollider2D poly_collider;

    void Start() {
        line_renderer = gameObject.GetComponent<LineRenderer>();

        poly_collider = gameObject.GetComponent<PolygonCollider2D>();
        poly_collider.enabled = false;

        Vector3[] col_points = new Vector3[4];
        Vector3[] line_points = new Vector3[2];
        line_renderer.GetPositions(line_points);

        Vector2 perp_vec = new Vector2(-1*(line_points[0].y-line_points[1].y),(line_points[0].x-line_points[1].x));
        perp_vec.Normalize();
        perp_vec *= width/2;

        Vector2 local_end = (line_points[1]-line_points[0]);
        local_end.Normalize();
        local_end *= FULL_LENGTH;
        line_renderer.SetPosition(1,(Vector2)line_points[0]+local_end);

        Vector2[] points = new Vector2[4];
        points[0] = perp_vec;
        points[1] = perp_vec * -1;
        points[2] = local_end - perp_vec;
        points[3] = local_end + perp_vec;
        poly_collider.SetPath(0, points);
    }

    void FixedUpdate()
    {
        if (delay <= 0f) {
            if (lifetime <= 0f) {
                poly_collider.enabled = false;
                if(line_renderer.widthMultiplier >= 0f) {
                    line_renderer.widthMultiplier -= 0.05f;
                }
                else {
                    if (destroy_source && source != null) {
                        Destroy(source);
                    }
                    Destroy(gameObject);
                }
            }
            else {
                poly_collider.enabled = true;
                if (line_renderer.widthMultiplier <= width) {
                    line_renderer.widthMultiplier += 0.05f;
                }
                lifetime -= Time.fixedDeltaTime;
            }
        }
        else {
            delay -= Time.fixedDeltaTime;
        }
    }
}
