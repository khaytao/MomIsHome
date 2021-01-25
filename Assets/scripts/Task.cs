using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TaskType
{
    None = 0,
    Fire = 1,
    Trash = 2,
    Repair = 3,
    Sweep = 4,
    Tape = 5,
    Furniture = 6,
    HoldingBin = 20,
}

public enum FurnitureType
{
    None = 0,
    Fridge = 1,
    Sink = 2,
    SofaVertical = 3,
    Stove = 4,
    Toilet = 5,
    Mat = 6,
    Lamp = 7,
}

public class Task : MonoBehaviour, IComparable<Task>
{
    public float duration;
    public TaskType type;
    public FurnitureType furnitureType;
    private SpriteRenderer childWaterRenderer;
    private bool isInteractable;
    private Animator animator;
    private bool isBurning;
    private int brokenLevel;
    [HideInInspector] public bool isFurniture;
    [HideInInspector] public Collider2D taskCollider;
    [HideInInspector] public Collider2D circleCollider;
    [HideInInspector] public SpriteRenderer taskRenderer;
    void Start()
    {
        brokenLevel = 0;
        isBurning = false;
        isFurniture = type == TaskType.Furniture;
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
        else if (isFurniture)
        {
            animator.SetInteger("Furniture", (int)furnitureType);
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

    public bool canFix(Item item)
    {
        if (!item)
            return false;
        
        // furniture has been damaged
        if (isFurniture && type != TaskType.Furniture)
        {
            // cant fix if fire is on it
            Bounds taskBounds = taskCollider.bounds;
            if (GameManager.Instance.isInFire(taskBounds))
                return false;
        }
        return type == item.forTaskType;
    }

    public Item finishFix(Item item)
    {
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
            GameManager.Instance.FinishTask(this);
        }
        else if (type == TaskType.Tape)
        {
            brokenLevel--;
            if (brokenLevel == 0)
                type = TaskType.Furniture;
            animator.SetInteger("BrokenLevel", brokenLevel);
            GameManager.Instance.FinishTask(this);
        }
        else if (type == TaskType.Fire)
        {
            GameManager.Instance.removeFire(gameObject);
            GameManager.Instance.FinishTask(this);
            // todo: instantiate ash prefab
        }
        else
        {
            GameManager.Instance.removeTask(this);
            Destroy(gameObject);
            GameManager.Instance.FinishTask(this);
        }
        
        return item;
    }

    public void triggerInteractable(bool interactable)
    {
        isInteractable = interactable;
        animator.SetBool("Interactable", interactable);
    }

    public void burnFurniture()
    {
        // some furniture don't have 2 broken levels?
        if (brokenLevel < 2)
        {
            brokenLevel++;
            // todo: how to make damaged and how to fix? make method 'can fix'
            animator.SetInteger("BrokenLevel", 1);
            type = TaskType.Tape;
            GameManager.Instance.addToTaskCount(1);
        }
    }

    public int CompareTo(Task other)
    {
        Vector3 playerPos = GameManager.Instance.getPlayerScript().gameObject.transform.position;
        float dist1 = Vector3.Distance(playerPos, gameObject.transform.position);
        float dist2 = Vector3.Distance(playerPos, other.gameObject.transform.position);
        return dist1 == dist2 ? 0 : (dist1 < dist2 ? -1 : 1);
    }
}
