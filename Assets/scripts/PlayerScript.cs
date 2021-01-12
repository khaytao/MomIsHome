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
    private ArrayList curTasks;
    private Task fixingTask;
    private float taskStarted;
    private bool isFixing;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        curTasks = new ArrayList();
        GameManager.Instance.setPlayerScript(this);
        _goal = _rb.position;
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
        if (Input.GetKeyDown(interactKey) && !isFixing && curTasks.Count > 0 && holdingItem != null)
        {
            foreach (Task task in curTasks)
            {
                if (task.type == holdingItem.forTaskType)
                {
                    fixingTask = curTasks[0] as Task;
                    doTask();
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(interactKey))
        {
            if(holdingItem != null)
            {
                holdingItem.transform.parent = null;
                holdingItem = null;
            }
            else if (curItem != null)
            {
                holdingItem = curItem;
                holdingItem.gameObject.transform.parent = gameObject.transform;
            }
        }
    }

    private void doTask()
    {
        if (fixingTask == null)
        {
            isFixing = false;
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
        float elapsedPerc = (Time.time - taskStarted) / fixingTask.duration;
        //progressBar.value = elapsedPerc;

        if (elapsedPerc >= 1)
        {
            isFixing = false;
            _animator.SetBool("isWorking", false);
            //progressBar.enabled = false;
            if (fixingTask.type == TaskType.Trash)
            {
                GameManager.Instance.removeItem(holdingItem);
                Destroy(holdingItem.gameObject);
                GameManager.Instance.FinishTask(fixingTask);
                holdingItem = null;
            }
            else
            {
                GameManager.Instance.removeTask(fixingTask);
                curTasks.Remove(fixingTask);
                Destroy(fixingTask.gameObject);
                GameManager.Instance.FinishTask(fixingTask);
                fixingTask = null;
            }
            //Destroy(holdingItem.gameObject);
            //holdingItem.transform.parent = null;
            //holdingItem = null;
            //GameManager.Instance.areTasksOver();

        }
    }


    void FixedUpdate()
    {
        Vector2 direction = _goal - _rb.position;
        
        // keyboard input
        direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }
        
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

    public void addToTask(Task task)
    {
        curTasks.Add(task);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Item"))
        {
            curItem = GameManager.Instance.getItem(collision.gameObject);
        }
        if (collision.tag.Equals("Task"))
        {
            Task colTask = GameManager.Instance.getTask(collision.gameObject);
            if(!curTasks.Contains(colTask))
                curTasks.Add(colTask);
            // curTask = GameManager.Instance.getTask(collision.gameObject);
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
            Task task = GameManager.Instance.getTask(collision.gameObject);
            if(task != null)
                curTasks.Remove(task);
            // if(curTask != null && curTask.gameObject.Equals(collision.gameObject))
            //     curTask = null;
        }
    }
}