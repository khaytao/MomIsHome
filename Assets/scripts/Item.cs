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

    void Start()
    {
        isInteractable = false;
        itemCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        MyGameManager.Instance.addItem(this);
    }

    public void triggerInteractable(bool interactable)
    {
        isInteractable = interactable;
        animator.SetBool("Interactable", interactable);
    }
}
