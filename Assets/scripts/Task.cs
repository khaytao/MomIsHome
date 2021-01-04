using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public float duration;
    public Color color;
    [HideInInspector] public Collider2D taskCollider;
    [HideInInspector] public SpriteRenderer taskRenderer;
    void Start()
    {
        
        taskCollider = GetComponent<Collider2D>();
        taskRenderer = GetComponent<SpriteRenderer>();
        color = taskRenderer.color;
        GameManager.Instance.addTask(this);
    }
}
