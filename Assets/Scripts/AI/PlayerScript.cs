using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Range(1, 4)]
    public int SimFires = 4;
    
    public Slider progressBar;
    public KeyCode interactKey;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer playerRenderer;
    private Vector2 _goal;
    public float goal_threshold;
    public float speed;
    public float mashingReduce;
    private float _angle = 0;
    private Item holdingItem;
    [HideInInspector] public Task holdingBin;
    private Item curItem;
    private List<Task> curTasks;
    private List<Task> fixingTasks;
    private float taskStarted;
    [HideInInspector] public bool isFixing;
    private List<Task> highlightedTasks;
    private Item highlightedItem;
    private Vector3 puddleScaleTaskStart;

    public float wallBounce = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        highlightedTasks = new List<Task>();
        _rb = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<SpriteRenderer>();
        //_animator = GetComponent<Animator>();
        curTasks = new List<Task>();
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
        /*if (Input.GetMouseButtonDown(0))
        {
            _goal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }*/

        if (Input.GetKeyDown(interactKey))
        {
            handleInteract();
        }
    }

    private void handleInteract()
    {
        if (Input.GetKeyDown(interactKey) && highlightedTasks.Count > 0 && highlightedTasks[0].canFix(holdingItem))
        {
            fixingTasks = new List<Task>(highlightedTasks);
            doTask();
            return;
        }
        
        
        if (holdingBin)
        {
            AudioManager.i.PlaySound(AudioFileGetter.i.drop);
            holdingBin.transform.parent = transform.parent;;
            holdingBin.gameObject.SetActive(true);
            holdingBin = null;
            _animator.SetInteger("HoldingItem", 0);
            updateHighlighted();
            return;
        }
        
        
        if (holdingItem)
        {
            AudioManager.i.PlaySound(AudioFileGetter.i.drop);
            _animator.SetInteger("HoldingItem", 0);
            holdingItem.gameObject.SetActive(true);
            holdingItem.transform.parent = transform.parent;
            holdingItem = null;
        }
        
        if(highlightedTasks.Count > 0 && highlightedTasks[0].type == TaskType.Trash)
        {
            AudioManager.i.PlaySound(AudioFileGetter.i.pick);
            holdingBin = highlightedTasks[0];
            holdingBin.gameObject.transform.parent = gameObject.transform;
            // 20 for holding bin in animator
            _animator.SetInteger("HoldingItem", 20);
            holdingBin.gameObject.SetActive(false);
        }

        
        if (!holdingBin && highlightedItem)
        {
            AudioManager.i.PlaySound(AudioFileGetter.i.pick);
            holdingItem = highlightedItem;
            holdingItem.gameObject.transform.parent = gameObject.transform;
            _animator.SetInteger("HoldingItem", (int) holdingItem.forTaskType);
            holdingItem.gameObject.SetActive(false);
            highlightedItem = null;
        }
        updateHighlighted();
    }

    private void doTask()
    {
        if (fixingTasks.Count == 0)
        {
            isFixing = false;
            return ;
        }
        if (!isFixing)
        {
            playFixSound();
            _animator.SetBool("isMoving", false);
            isFixing = true;
            taskStarted = Time.time;
            progressBar.gameObject.SetActive(true);
            if (fixingTasks[0].type == TaskType.Sweep)
                puddleScaleTaskStart = fixingTasks[0].transform.localScale;
        }
        float elapsedPerc = (Time.time - taskStarted) / fixingTasks[0].duration;
        // puddle no longer in
        if (fixingTasks[0].type == TaskType.Sweep)
        {
            if (!fixingTasks[0].taskRenderer.bounds.Intersects(playerRenderer.bounds))
            {
                fixingTasks[0].duration *= (1 - elapsedPerc);
                isFixing = false;
                _animator.SetBool("isWorking", false);
                progressBar.gameObject.SetActive(false);
                fixingTasks.Clear();
                return;
            }
        }

        // make puddle smaller
        if (fixingTasks[0].type == TaskType.Sweep)
        {
            // goal scale is the initial scale in the inspector
            fixingTasks[0].transform.localScale =  puddleScaleTaskStart - elapsedPerc * (puddleScaleTaskStart - new Vector3(0.045f, 0.045f));
        }

        _animator.SetBool("isWorking", true);
        progressBar.value = elapsedPerc;

        if (elapsedPerc >= 1)
        {
            isFixing = false;
            _animator.SetBool("isWorking", false);
            progressBar.gameObject.SetActive(false);
            
            foreach (var task in fixingTasks)
            {
                holdingItem = task.finishFix(holdingItem);
            }
            fixingTasks.Clear();
            if (holdingItem == null)
            {
                _animator.SetInteger("HoldingItem", 0);
            }
            updateHighlighted();
        }
    }

    private void makeContinousSound(AudioClip c, float delay)
    {
        AudioManager.i.playerActionSound(c);
        StartCoroutine(stopSound(delay));
    }
    
    private void playFixSound()
    {
        TaskType type = fixingTasks[0].type;
        if (type == TaskType.Trash)
        {
            AudioManager.i.PlaySound(AudioFileGetter.i.trash);
            // TODO: KFIR - THROWING TRASH
        }
        else if (type == TaskType.NPC)
        {
            // TODO: KFIR - KICKING OUT NPC
            AudioManager.i.PlaySound(AudioFileGetter.i.angry);
        }

        else if (type == TaskType.Sweep)
        {
            makeContinousSound(AudioFileGetter.i.mop, fixingTasks[0].duration);
        }
        
        else if (type == TaskType.Fire)
        {
            makeContinousSound(AudioFileGetter.i.fireEx, fixingTasks[0].duration);
        }
        else if (type == TaskType.Furniture)
        {
            makeContinousSound(AudioFileGetter.i.fix, fixingTasks[0].duration);
        }
        
    }

    private IEnumerator stopSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.i.stopPlayerActionSound();
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

        if (_rb.velocity.x > 0.01f && !playerRenderer.flipX)
        {
            playerRenderer.flipX = true;
        }

        if (_rb.velocity.x < -0.01f && playerRenderer.flipX)
        {
            playerRenderer.flipX = false;
        }
        if (_rb.velocity.magnitude == 0)
        {
            _animator.SetBool("isMoving", false);
            if (!isFixing)
            {
                AudioManager.i.stopPlayerActionSound();
            }
            
        }
        else
        {
            _animator.SetBool("isMoving", true);
            AudioManager.i.playerActionSound(AudioFileGetter.i.run);
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
        updateHighlighted();
    }
    public void addToTask(Task task)
    {
        curTasks.Add(task);
        updateHighlighted();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag.Equals("Item"))
        {
            curItem = GameManager.Instance.getItem(collision.gameObject);
            updateHighlighted();
        }
        if (collision.tag.Equals("Task"))
        {
            Task colTask = GameManager.Instance.getTask(collision.gameObject);
            if(!curTasks.Contains(colTask))
                curTasks.Add(colTask);
            updateHighlighted();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Item") && curItem != null && curItem.gameObject.Equals(collision.gameObject))
        {
            curItem = null;
            updateHighlighted();
        }
        if (collision.tag.Equals("Task"))
        {
            Task task = GameManager.Instance.getTask(collision.gameObject);
            if(task != null)
                curTasks.Remove(task);
            updateHighlighted();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Wall"))
        {
            _rb.AddForce(wallBounce * _rb.velocity.normalized);
        }
    }

    private void updateHighlighted()
    {
        List<Task> tasks = new List<Task>();
        
        // clear interactables
        foreach (Task task in highlightedTasks)
            if(task)
                task.triggerInteractable(false);
        highlightedTasks.Clear();
        if (highlightedItem)
        {
            highlightedItem.triggerInteractable(false);
            highlightedItem = null;
        }
        
        // priority to fixing tasks
        foreach (Task task in curTasks)
            if (task != null && (task.type == TaskType.Trash || task.canFix(holdingItem)))
                tasks.Add(task);

        tasks.Sort();
        if (tasks.Count == 0 && !curItem)
            return;

        if (curItem && (tasks.Count == 0 || isCloser(curItem.gameObject, tasks[0].gameObject)))
        {
            highlightedItem = curItem;
            highlightedItem.triggerInteractable(true);
            return;
        }

        if (highlightedItem)
        {
            highlightedItem.triggerInteractable(false);
            highlightedItem = null;
        }
        highlightedTasks.Add(tasks[0]);
        if (highlightedTasks[0].type == TaskType.Fire)
        {
            int fires = 1;
            for(int i=1; i<tasks.Count; i++)
                if (tasks[i].type == TaskType.Fire)
                {
                    fires++;
                    highlightedTasks.Add(tasks[i]);
                    if (fires >= SimFires)
                        break;
                }
        }

        foreach (var task in highlightedTasks)
            task.triggerInteractable(true);
        
    }

    private bool isCloser(GameObject go1, GameObject go2)
    {
        float dist1 = Vector3.Distance(gameObject.transform.position, go1.transform.position);
        float dist2 = Vector3.Distance(gameObject.transform.position, go2.transform.position);
        return dist1 < dist2;
    }
}