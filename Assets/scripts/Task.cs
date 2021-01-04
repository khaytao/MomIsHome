using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType
{
    type1,
    type2,
    type3
}

public class Task : MonoBehaviour
{
    public float duration;

    public TaskType type;
    [HideInInspector] public Collider2D taskCollider;
    [HideInInspector] public SpriteRenderer taskRenderer;
    public string fixedBy = "Nothing";
    void Start()
    {
        
        taskCollider = GetComponent<Collider2D>();
        taskRenderer = GetComponent<SpriteRenderer>();
        GameManager.Instance.addTask(this);
    }
}
