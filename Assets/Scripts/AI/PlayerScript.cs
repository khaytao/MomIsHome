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
    public Animator _animator;
    private SpriteRenderer playerRenderer;
    private Vector2 _goal;
    public float goal_threshold;
    public float speed;
    private float _angle = 0;
    private Item holdingItem;
    private Task holdingBin;
    private Item curItem;
    private ArrayList curTasks;
    private Task fixingTask;
    private float taskStarted;
    private bool isFixing;
    private bool isHoldingBin;

    public float wallBounce = 1f;
    public Text tx;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<SpriteRenderer>();
        //_animator = GetComponent<Animator>();
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
        }
        if (Input.GetKeyDown(interactKey) && !isFixing && curTasks.Count > 0 && holdingItem != null)
        {
            foreach (Task task in curTasks)
            {
                if (task.type == holdingItem.forTaskType)
                {
                    fixingTask = task;
                    doTask();
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(interactKey))
        {
            if (holdingBin != null)
            {
                holdingBin.transform.parent = null;
                holdingBin.gameObject.SetActive(true);
                holdingBin = null;
                _animator.SetInteger("HoldingItem", 0);
            }
            else
            {
                if (holdingItem != null)
                {
                    _animator.SetInteger("HoldingItem", 0);
                    holdingItem.gameObject.SetActive(true);
                    holdingItem.transform.parent = null;
                    holdingItem = null;
                }
                foreach (Task task in curTasks)
                {
                    if (task.type == TaskType.Trash)
                    {
                        holdingBin = task;
                        holdingBin.gameObject.transform.parent = gameObject.transform;
                        // 20 for holding bin in animator
                        _animator.SetInteger("HoldingItem", 20);
                        holdingBin.gameObject.SetActive(false);
                        break;
                    }
                }

                if (holdingBin == null && curItem != null)
                {
                    holdingItem = curItem;
                    holdingItem.gameObject.transform.parent = gameObject.transform;
                    _animator.SetInteger("HoldingItem", (int) holdingItem.forTaskType);
                    holdingItem.gameObject.SetActive(false);
                }
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
            progressBar.gameObject.SetActive(true);
        }

        _animator.SetBool("isWorking", true);
        float elapsedPerc = (Time.time - taskStarted) / fixingTask.duration;
        progressBar.value = elapsedPerc;

        if (elapsedPerc >= 1)
        {
            isFixing = false;
            _animator.SetBool("isWorking", false);
            progressBar.gameObject.SetActive(false);

            Tuple<Task, Item> result = fixingTask.finishFix(holdingItem);
            fixingTask = result.Item1;
            holdingItem = result.Item2;
            if (holdingItem == null)
            {
                _animator.SetInteger("HoldingItem", 0);
            }
        }
    }


    void FixedUpdate()
    {
        if (isFixing)
        {
            _rb.velocity = Vector2.zero;
            return;
        }
        Vector2 direction = _goal - _rb.position;
        
        // keyboard input
        direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector2.up;
        }
        
        if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector2.left;
        }
        
        if (Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector2.down;
        }
        
        if (Input.GetKey(KeyCode.D)|| Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector2.right;
        }
        
        _rb.velocity = speed * direction.normalized;

        if (!isFixing && direction.magnitude < goal_threshold)
        {
            _rb.velocity = Vector2.zero;
        }

        if (_rb.velocity.x > 0 && !playerRenderer.flipX)
        {
            playerRenderer.flipX = true;
        }

        if (_rb.velocity.x < 0 && playerRenderer.flipX)
        {
            playerRenderer.flipX = false;
        }
        if (_rb.velocity.magnitude == 0)
        {
            _animator.SetBool("isMoving", false);
        }
        else
        {
            _animator.SetBool("isMoving", true);
        }
        
        _angle = Vector2.Angle(direction, -transform.up);
        if (_rb.velocity.x < 0)
        {
            _angle = 360 - _angle;
        }
        _animator.SetFloat("angle", _angle);
    }

    public void removeTask(Task task)
    {
        if(curTasks.Contains(task))
            curTasks.Remove(task);
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
        if (collision.tag.Equals("Item") && curItem != null && curItem.gameObject.Equals(collision.gameObject))
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Wall"))
        {
            _rb.AddForce(wallBounce * _rb.velocity.normalized);
        }
    }
}