﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


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
    public NPCType NPCType;
    private bool isInteractable;
    private Animator animator;
    private bool isBurning;
    private bool isBroken;
    private FountainScript fountainScript;
    private GameObject fountainGO;
    private Vector2 originalSize; // size of the renderer bounds without highlight

    [HideInInspector] public Vector3 initialScale;
    [HideInInspector] public float initialDuration;
    [HideInInspector] public FireScript fireScript;
    [HideInInspector] public EnemyAI enemyAI;
    [HideInInspector] public bool isFurniture;
    [HideInInspector] public bool isNPC;
    [HideInInspector] public Collider2D taskCollider;
    [HideInInspector] public Collider2D interactCollider;
    [HideInInspector] public CircleCollider2D circleCollider;
    [HideInInspector] public SpriteRenderer taskRenderer;
    [HideInInspector] public bool isActive;

    [Range(0, 10)]
    public int burnChance;
    void Start()
    {
        isBurning = false;
        isBroken = false;
        fountainScript = null;
        isFurniture = type == TaskType.Furniture;
        isNPC = type == TaskType.NPC;
        isInteractable = false;
        taskCollider = GetComponent<Collider2D>();
        taskRenderer = GetComponent<SpriteRenderer>();
        originalSize = taskRenderer.bounds.size;
        if (type == TaskType.Sweep)
            interactCollider = GetComponent<BoxCollider2D>();
        else
            interactCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        if(type == TaskType.Fire)
            fireScript = GetComponent<FireScript>();
        
        MyGameManager.Instance.addTask(this);

        duration = duration / 2;
        
        initialDuration = duration;

        if (isFurniture)
        {
            animator.SetInteger("Furniture", (int) furnitureType);
            if (canLeak())
            {
                fountainScript = gameObject.AddComponent<FountainScript>();
                fountainGO = Instantiate(Resources.Load("WaterFountain"), transform.position, Quaternion.identity) as GameObject;
                Bounds bounds = fountainGO.GetComponent<SpriteRenderer>().bounds;
                fountainGO.transform.position += new Vector3(0, bounds.extents.y/3);
                fountainGO.SetActive(false);
                fountainScript.enabled = false;
            }
        }

        if (isNPC)
        {
            animator.SetInteger("NPC", (int) NPCType);
            enemyAI = GetComponent<EnemyAI>();
        }
        
        initForLevel();
    }

    public bool canFix(Item heldItem)
    {
        if (type == TaskType.NPC && !heldItem && !MyGameManager.Instance.getPlayerScript().holdingBin)
            return true;

        if (!heldItem)
            return false;
        
        // furniture has been damaged
        if (isFurniture && type != TaskType.Furniture)
        {
            // cant fix if fire is on it
            Bounds taskBounds = taskCollider.bounds;
            if (MyGameManager.Instance.isInFire(taskBounds))
                return false;
        }
        return type == heldItem.forTaskType;
    }

    public Item finishFix(Item item)
    {
        if (type == TaskType.Trash)
        {
            // 80% to burn
            if (item.canBurn && UnityEngine.Random.Range(0, 10) < burnChance)
            {
                type = TaskType.Fire;
                animator.SetBool("Burning", true);
                isBurning = true;
                MyGameManager.Instance.addToTaskCount(1);
                MyGameManager.Instance.TrashCanOnFire = true;
            }
            MyGameManager.Instance.removeItem(item);
            // Destroy(item.gameObject);
            MyGameManager.Instance.FinishTask(this);
            item.gameObject.transform.parent = null;
            item = null;
        }
        else if (type == TaskType.Fire && isBurning)
        {
            type = TaskType.Trash;
            animator.SetBool("Burning", false);
            MyGameManager.Instance.TrashCanOnFire = false;
            MyGameManager.Instance.FinishTask(this);
        }
        else if (type == TaskType.Tape && isFurniture)
        {
            if (canLeak())
            {
                fountainGO.SetActive(false);
                fountainScript.resetForLevel();
                fountainScript.enabled = false;
                MyGameManager.Instance.leakCount--;
            }

            type = TaskType.Furniture;
            animator.SetInteger("BrokenLevel", 0);
            MyGameManager.Instance.FinishTask(this);
        }
        else if (type == TaskType.Fire)
        {
            MyGameManager.Instance.removeFireBounds(gameObject);
            MyGameManager.Instance.removeTask(this);
            MyGameManager.Instance.FinishTask(this);
        }

        else if (type == TaskType.NPC)
        {
            // MyGameManager.Instance.removeTask(this);
            MyGameManager.Instance.FinishTask(this);
            enemyAI.Leave();
        }
        else
        {
            MyGameManager.Instance.removeTask(this);
            // Destroy(gameObject);
            MyGameManager.Instance.FinishTask(this);
        }
        
        return item;
    }

    public void triggerInteractable(bool interactable)
    {
        isInteractable = interactable;
        animator.SetBool("Interactable", interactable);
    }

    // get the bounds of the task without highlight
    public Bounds getOriginalSizeBounds()
    {
        return new Bounds(transform.position, originalSize);
        // return interactCollider.bounds;
    }

    public Bounds getInteractBounds()
    {
        return interactCollider.bounds;
    }

    public void breakFurniture()
    {
        if (isBurning)
            return;

        if (canLeak())
        {
            MyGameManager.Instance.leakCount++;
            fountainScript.enabled = true;
            fountainGO.SetActive(true);
        }
        
        animator.SetInteger("BrokenLevel", 1);
        type = TaskType.Tape;
        MyGameManager.Instance.addToTaskCount(!isBroken ? 1 : 0);
        isBroken = true;
    }

    public void burnFurniture()
    {
        if (!canBurn())
            return;
        
        animator.SetInteger("BrokenLevel", 2);
        type = TaskType.Tape;
        MyGameManager.Instance.addToTaskCount(!isBroken && !isBurning ? 1 : 0);
        isBurning = true;
    }

    public bool canBreak()
    {
        return isFurniture && !isBroken && !isBurning;
    }

    public int CompareTo(Task other)
    {
        Vector3 playerPos = MyGameManager.Instance.getPlayerScript().gameObject.transform.position;
        float dist1 = Vector3.Distance(playerPos, gameObject.transform.position);
        float dist2 = Vector3.Distance(playerPos, other.gameObject.transform.position);
        return dist1 == dist2 ? 0 : (dist1 < dist2 ? -1 : 1);
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (type == TaskType.NPC && other.gameObject.CompareTag("Task"))
    //     {
    //         Task sc = other.gameObject.GetComponent<Task>();
    //         if (sc.isFurniture)
    //         {
    //             Debug.Log("broke" + other.gameObject.name);
    //         }
    //     }
    // }

    private bool canLeak()
    {
        return furnitureType == FurnitureType.Sink || furnitureType == FurnitureType.Toilet ||
               furnitureType == FurnitureType.Tub;
    }

    private bool canBurn()
    {
        return furnitureType == FurnitureType.BedRed || furnitureType == FurnitureType.Chair ||
               furnitureType == FurnitureType.Desk || furnitureType == FurnitureType.Lamp ||
               furnitureType == FurnitureType.MatBlue || furnitureType == FurnitureType.MatBlueSmall ||
               furnitureType == FurnitureType.MatCircle || furnitureType == FurnitureType.MatGreen ||
               furnitureType == FurnitureType.MatRed || furnitureType == FurnitureType.Microwave ||
               furnitureType == FurnitureType.SofaHor || furnitureType == FurnitureType.SofaVertical ||
               furnitureType == FurnitureType.Stove || furnitureType == FurnitureType.Table ||
               furnitureType == FurnitureType.Towel || furnitureType == FurnitureType.TV ||
               furnitureType == FurnitureType.ShowerCurtain;
    }

    public void resetForLevel()
    {
        if (isFurniture)
        {
            animator.SetInteger("BrokenLevel", 0);
            type = TaskType.Furniture;
            if (canLeak())
            {
                fountainGO.SetActive(false);
                fountainScript.enabled = false;
            }
        }
        animator.SetBool("Interactable", false);
        // trash can burning
        if (type == TaskType.Fire && isBurning)
        {
            type = TaskType.Trash;
        }

        if (type == TaskType.Fire)
        {
            fireScript.enabled = false;
        }
        
        if(canLeak())
            fountainScript.resetForLevel();

        if (isNPC)
        {
            enemyAI.resetForLevel();
            enemyAI.enabled = false;
        }

        isBroken = false;
        isBurning = false;

        if (!isFurniture)
        {
            gameObject.transform.position = MyGameManager.Instance.oblivion;
            isActive = false;
            gameObject.SetActive(false);
        }
    }
    
    public void initForLevel()
    {
        if (fireScript && type == TaskType.Fire)
            fireScript.enabled = true;

        isActive = true;
        duration = initialDuration;
        
        gameObject.SetActive(true);
        
        if (isNPC)
        {
            animator.SetInteger("NPC", (int) NPCType);
            if (enemyAI)
            {
                enemyAI.enabled = true;
                enemyAI.initForLevel();
            }

        }
    }

    private void OnDestroy()
    {
        if(fountainGO)
            Destroy(fountainGO);
    }
}
