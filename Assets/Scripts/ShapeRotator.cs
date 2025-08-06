using UnityEngine;

public class ShapeRotator : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    [SerializeField] bool clockwise = true;
    
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, rotateSpeed * Time.deltaTime * (clockwise ? 1 : -1)));
    }
}
