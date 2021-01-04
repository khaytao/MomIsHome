using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public Slider progressBar;
    public KeyCode interactKey;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _goal;
    public float goal_threshold;
    public float speed;
    private float _angle = 0;
    private Item holdingItem;
    private Item curItem;
    private Task curTask;
    private float taskStarted;
    private bool isFixing;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isFixing)
        {
            doTask();
            _goal = _rb.position;
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _goal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _animator.SetBool("isMoving", true);
        }
        if (Input.GetKeyDown(interactKey) && !isFixing && curTask != null && holdingItem != null)
        {
            doTask();
        }
        else if (Input.GetKeyDown(interactKey))
        {
            if(holdingItem != null)
            {
                holdingItem.transform.parent = null;
                holdingItem = null;
            }
            if (curItem != null)
            {
                holdingItem = curItem;
                holdingItem.gameObject.transform.parent = gameObject.transform;
            }
        }
    }

    private void doTask()
    {
        if (holdingItem.forTaskType != curTask.type)
        {
            return ;
        }
        if (!isFixing)
        {
            _animator.SetBool("isMoving", false);
            isFixing = true;
            taskStarted = Time.time;
            //progressBar.enabled = true;
        }

        _animator.SetBool("isWorking", true);
        float elapsedPerc = (Time.time - taskStarted) / curTask.duration;
        //progressBar.value = elapsedPerc;

        if (elapsedPerc >= 1)
        {
            isFixing = false;
            _animator.SetBool("isWorking", false);
            //progressBar.enabled = false;
            Destroy(curTask.gameObject);
            //Destroy(holdingItem.gameObject);
            curTask = null;
            holdingItem.transform.parent = null;
            holdingItem = null;
            
        }
    }


    void FixedUpdate()
    {
        Vector2 direction = _goal - _rb.position;
        _rb.velocity = speed * direction.normalized;

        if (!isFixing && direction.magnitude < goal_threshold)
        {
            _rb.velocity = Vector2.zero;
            _animator.SetBool("isMoving", false);
        }

        _angle = Vector2.Angle(direction, -transform.up);

        if (_rb.velocity.x < 0)
        {
            _angle = 360 - _angle;
        }
        _animator.SetFloat("angle", _angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Item"))
        {
            curItem = GameManager.Instance.getItem(collision.gameObject);
        }
        if (collision.tag.Equals("Task"))
        {
            curTask = GameManager.Instance.getTask(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Item"))
        {
            curItem = null;
        }
        if (collision.tag.Equals("Task"))
        {
            curTask = null;
        }
    }
}