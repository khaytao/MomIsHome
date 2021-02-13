using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour, IComparable<Item>
{
    public TaskType forTaskType;
    private Animator animator;
    private bool isInteractable;
    public bool canBurn;
    private Vector2 originalSize;
    [HideInInspector] public Collider2D itemCollider;
    [HideInInspector] public Collider2D interactCollider;
    [HideInInspector] public SpriteRenderer itemRenderer;
    [HideInInspector] public bool isActive;

    void Start()
    {
        isInteractable = false;
        itemCollider = GetComponent<Collider2D>();
        itemRenderer = GetComponent<SpriteRenderer>();
        interactCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        originalSize = itemRenderer.bounds.size;
        MyGameManager.Instance.addItem(this);
        
        initForLevel();
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
    
    public int CompareTo(Item other)
    {
        Vector3 playerPos = MyGameManager.Instance.getPlayerScript().gameObject.transform.position;
        float dist1 = Vector3.Distance(playerPos, gameObject.transform.position);
        float dist2 = Vector3.Distance(playerPos, other.gameObject.transform.position);
        return dist1 == dist2 ? 0 : (dist1 < dist2 ? -1 : 1);
    }

    public void initForLevel()
    {
        isActive = true;
        gameObject.SetActive(true);
    }

    public void resetForLevel()
    {
        gameObject.transform.position = MyGameManager.Instance.oblivion;
        isActive = false;
        gameObject.SetActive(false);
    }
}
