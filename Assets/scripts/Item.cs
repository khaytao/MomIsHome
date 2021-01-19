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
        GameManager.Instance.addItem(this);
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
    
    public void triggerInteractable()
    {
        isInteractable = !isInteractable;
        animator.SetBool("Interactable", isInteractable);
    }
}
