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
    NPC = 7,
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
    MatRed = 6,
    Lamp = 7,
    BedRed = 8,
    MatBlue = 9,
    MatBlueSmall = 10,
    MatGreen = 11,
    MatCircle = 12,
    Tub = 13,
    BedBlue = 14,
    Table = 15,
    PotSmall = 16,
    PotMed = 17,
    PotBig = 18,
    Chair = 19,
    Desk = 20,
    SofaHor = 21,
    Washer = 22,
    TV = 23,
    ShowerCurtain = 24,
    PC = 25,
    Towel = 26,
    Mirror = 27,
    Microwave = 28,
}

public enum NPCType
{
    None = 0,
    DrunkFriend = 1,
    Hobo = 2,
    Racoon = 3,
}

public class Task : MonoBehaviour, IComparable<Task>
{
    public float duration;
    public TaskType type;
    public FurnitureType furnitureType;
    public int furnitureInitBreakLevel;
    public NPCType NPCType;
    private bool isInteractable;
    private Animator animator;
    private bool isBurning;
    private bool isBroken;
    
    [HideInInspector] public bool isFurniture;
    [HideInInspector] public bool isNPC;
    [HideInInspector] public Collider2D taskCollider;
    [HideInInspector] public Collider2D circleCollider;
    [HideInInspector] public SpriteRenderer taskRenderer;
    void Start()
    {
        isBurning = false;
        isBroken = false;
        isFurniture = type == TaskType.Furniture;
        isNPC = type == TaskType.NPC;
        isInteractable = false;
        taskCollider = GetComponent<Collider2D>();
        taskRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        GameManager.Instance.addTask(this);

        if (isFurniture)
        {
            animator.SetInteger("Furniture", (int) furnitureType);
            if (furnitureInitBreakLevel == 1)
                breakFurniture();
            else if(furnitureInitBreakLevel == 2)
                burnFurniture();
        }

        if (isNPC)
        {
            animator.SetInteger("NPC", (int) NPCType);
        }
    }
    
    public bool canFix(Item heldItem)
    {
        if (type == TaskType.NPC && !heldItem && !GameManager.Instance.getPlayerScript().holdingBin)
            return true;

        if (!heldItem)
            return false;
        
        // furniture has been damaged
        if (isFurniture && type != TaskType.Furniture)
        {
            // cant fix if fire is on it
            Bounds taskBounds = taskCollider.bounds;
            if (GameManager.Instance.isInFire(taskBounds))
                return false;
        }
        return type == heldItem.forTaskType;
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
        else if (type == TaskType.Tape && isFurniture)
        {
            type = TaskType.Furniture;
            animator.SetInteger("BrokenLevel", 0);
            furnitureInitBreakLevel = 0;
            GameManager.Instance.FinishTask(this);
        }
        else if (type == TaskType.Fire)
        {
            GameManager.Instance.removeFire(gameObject);
            GameManager.Instance.FinishTask(this);
            // todo: instantiate ash prefab
        }

        else if (type == TaskType.NPC)
        {
            GameManager.Instance.removeTask(this); //todo why this function?
            GameManager.Instance.FinishTask(this);
            EnemyAI ai = GetComponent<EnemyAI>();
            ai.Leave();
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

    public void breakFurniture()
    {
        if (isBurning)
            return;
        
        animator.SetInteger("BrokenLevel", 1);
        type = TaskType.Tape;
        GameManager.Instance.addToTaskCount(!isBroken ? 1 : 0);
        isBroken = true;
    }

    public void burnFurniture()
    {
        animator.SetInteger("BrokenLevel", 2);
        type = TaskType.Tape;
        GameManager.Instance.addToTaskCount(!isBroken && !isBurning ? 1 : 0);
        isBurning = true;
    }

    public int CompareTo(Task other)
    {
        Vector3 playerPos = GameManager.Instance.getPlayerScript().gameObject.transform.position;
        float dist1 = Vector3.Distance(playerPos, gameObject.transform.position);
        float dist2 = Vector3.Distance(playerPos, other.gameObject.transform.position);
        return dist1 == dist2 ? 0 : (dist1 < dist2 ? -1 : 1);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (type == TaskType.NPC && other.gameObject.CompareTag("Task"))
        {
            Task sc = other.gameObject.GetComponent<Task>();
            if (sc.isFurniture && sc.furnitureInitBreakLevel == 0)
            {
                Debug.Log("broke" + other.gameObject.name);
            }
        }
    }
}
