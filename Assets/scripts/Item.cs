using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    public TaskType forTaskType;
    private Animator animator;
    private bool isInteractable;
    public bool canBurn;
    [HideInInspector] public Collider2D itemCollider;
    [HideInInspector] public SpriteRenderer itemRenderer;
    [HideInInspector] public bool isActive;

    void Start()
    {
        isInteractable = false;
        itemCollider = GetComponent<Collider2D>();
        itemRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        MyGameManager.Instance.addItem(this);
        
        initForLevel();
    }

    public void triggerInteractable(bool interactable)
    {
        isInteractable = interactable;
        animator.SetBool("Interactable", interactable);
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
