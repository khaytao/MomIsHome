using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMovement : MonoBehaviour
{
    public Transform T;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        Debug.Log("click");
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        T.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            T.position = pos;
        }
    }
}
