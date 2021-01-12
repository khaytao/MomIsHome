using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType
{
    Fire,
    Trash,
    Repair,
    Sweep
}

public class Task : MonoBehaviour
{
    public float duration;
    public TaskType type;
    private SpriteRenderer childWaterRenderer;
    [HideInInspector] public Collider2D taskCollider;
    [HideInInspector] public Collider2D circleCollider;
    [HideInInspector] public SpriteRenderer taskRenderer;
    void Start()
    {
        taskCollider = GetComponent<Collider2D>();
        taskRenderer = GetComponent<SpriteRenderer>();
        GameManager.Instance.addTask(this);
        if (type == TaskType.Sweep)
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            childWaterRenderer = renderers[renderers.Length - 1];
        }
    }

    private void Update()
    {
        if (type == TaskType.Sweep)
        {
            childWaterRenderer.transform.localScale += Time.deltaTime * 0.1f * (Vector3)Vector2.one;
            duration += Time.deltaTime * 0.1f;
        }
    }
}
