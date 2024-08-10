using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public int stage;


    void Start()
    {
        stage = 0;
    }

    void Update()
    {
        if (Input.GetKey("escape")) {
            Application.Quit();
        }
    }

    public GameObject[] CreatePool(GameObject bul, int n) {
        GameObject[] new_pool = new GameObject[n];
        for (int i = 0; i < n; i ++) {
            new_pool[i] = Instantiate(bul, transform);
            new_pool[i].SetActive(false);
            new_pool[i].name = bul.name;
        }
        return new_pool;
    }
}
