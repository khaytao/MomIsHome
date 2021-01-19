using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TaskType
{
    Fire,
    Trash,
    Repair,
    Sweep,
    Tape,
}

public class Task : MonoBehaviour
{
    public float duration;
    public TaskType type;
    private SpriteRenderer childWaterRenderer;
    private bool isInteractable;
    private Animator animator;
    private bool isBurning;
    [HideInInspector] public Collider2D taskCollider;
    [HideInInspector] public Collider2D circleCollider;
    [HideInInspector] public SpriteRenderer taskRenderer;
    void Start()
    {
        isBurning = false;
        isInteractable = false;
        taskCollider = GetComponent<Collider2D>();
        taskRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
            childWaterRenderer.transform.localScale += Time.deltaTime * 0.05f * (Vector3)Vector2.one;
            duration += Time.deltaTime * 0.1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            animator.SetBool("Interactable", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            animator.SetBool("Interactable", false);
        }
    }

    public Tuple<Task, Item> finishFix(Item item)
    {
        Task task = this;
        if (type == TaskType.Trash)
        {
            // 80% to burn
            if (item.canBurn && UnityEngine.Random.Range(0, 10) < 8)
            {
                type = TaskType.Fire;
                animator.SetBool("Burning", true);
                isBurning = true;
                GameManager.Instance.addToTaskCount(1);
            }
            GameManager.Instance.removeItem(item);
            Destroy(item.gameObject);
            GameManager.Instance.FinishTask(this);
            item = null;
        }
        else if (type == TaskType.Fire && isBurning)
        {
            type = TaskType.Trash;
            animator.SetBool("Burning", false);
            GameManager.Instance.addToTaskCount(-1);
        }
        // else if (type == TaskType.Tape)
        // {
        //     // todo: restore sprite to its original state (animator?)
        // }
        // else if (type == TaskType.Fire)
        // {
        //     // todo: instantiate ash prefab
        // }
        else
        {
            GameManager.Instance.removeTask(this);
            Destroy(gameObject);
            GameManager.Instance.FinishTask(this);
            task = null;
        }
        
        return new Tuple<Task, Item>(task, item);
    }

    public void triggerInteractable()
    {
        isInteractable = !isInteractable;
        animator.SetBool("Interactable", isInteractable);
    }
}
