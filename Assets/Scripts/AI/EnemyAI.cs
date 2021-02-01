using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;


enum ActionType
{
    Idle = 0,
    Move = 1,
    Dirty = 2,
    Break = 3,
}

public class EnemyAI : MonoBehaviour
{
    public float thresh = 0.1f;
    public float dropOffset;
    public float timeBetweenPoints;
    public List<Transform> pointsList;
    public List<string> spawnList;
    public Animator anim;
    private AIPath path;
    private AIDestinationSetter ds;
    private Rigidbody2D rb;

    private bool leaving;
    private float movementThreshold = 0.1f;
    private int i;

    public Transform exit;
    public float x_limit;
    public float y_limit;
    private float startedAction; // time started
    private bool actionIsDirty;
    public float idleTimeBeforeAction;
    private bool isInAction;

    private Vector2 curPos;

    private float angleAnim;

    private SpriteRenderer renderer;
    private Task task;
    

    private bool isMoving;

    private bool updatingLocation;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        i = 0;
        ds = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        task = GetComponent<Task>();
        ds.target = pointsList[i];
        curPos = transform.position;
        renderer = GetComponent<SpriteRenderer>();
        startedAction = -1;
        isInAction = false;
        updatingLocation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!(Mathf.Abs(transform.position.x) < x_limit && Mathf.Abs(transform.position.y) < y_limit))
        {
            Destroy(gameObject);
        }

        if (isInAction)
        {
            path.canMove = false;
            StartCoroutine(doAction());
            return;
        }

        // updateAnimator();

        
        // do something
        if (!isInAction && !updatingLocation && pointsList[i] && Vector2.Distance(transform.position, pointsList[i].position) < thresh && !leaving)
        {
            i = (i + 1) % pointsList.Count;
            Invoke(nameof(updateLocation), timeBetweenPoints);
            startAction(true);
        }

        var nn = AstarPath.active.GetNearest(transform.position, NNConstraint.Default);
        var closestPointOnGraph = nn.clampedPosition;
        transform.position = (Vector2) closestPointOnGraph;
    }

    IEnumerator doAction()
    {
        if (startedAction >= 0)
        {
            startedAction = -1;
            anim.SetInteger("Action", (int) ActionType.Idle);
            yield return new WaitForSeconds(idleTimeBeforeAction);
            anim.SetInteger("Action", (int) ActionType.Dirty);
            AudioManager.i.PlaySound(AudioFileGetter.i.vomit);
            // wait for animation to end
            yield return new WaitForSeconds(18f / 24f);
            if (task.NPCType == NPCType.DrunkFriend || task.NPCType == NPCType.Hobo)
            {
                GameObject puddle = Instantiate(Resources.Load("Puddle")) as GameObject;
                Vector2 puddleExtents = puddle.GetComponent<SpriteRenderer>().bounds.extents;
                // calc puddle location
                int dirX = renderer.flipX ? 1 : -1;
                Vector2 rendExtents = renderer.bounds.extents;
                Vector3 newPos = new Vector3(transform.position.x + dirX * (rendExtents.x - puddleExtents.x * 1.5f), transform.position.y - rendExtents.y, transform.position.z);
                puddle.transform.position = newPos;
            }
            anim.SetInteger("Action", (int) ActionType.Idle);
            path.canMove = true;
            isInAction = false;
        }
    }

    private void startAction(bool actionDirty)
    {
        isInAction = true;
        startedAction = Time.time;
        actionIsDirty = actionDirty;
    }


    private void updateAnimator()
    {
        Vector2 pos = transform.position;
        Vector2 movementDirection = pos - curPos;

        curPos = pos;
        if (isMoving && (path.velocity == Vector3.zero || path.velocity.magnitude < movementThreshold))
        {
            isMoving = false;
            anim.SetInteger("Action", (int) ActionType.Idle);
        }
        else if (!isMoving && (path.velocity != Vector3.zero && path.velocity.magnitude > movementThreshold))
        {
            isMoving = true;
            anim.SetInteger("Action", (int) ActionType.Move);
        }

        angleAnim = Vector2.Angle(Vector2.up, movementDirection);
        if (path.velocity.x > 0.01f)
        {
            renderer.flipX = true;
        }

        if (path.velocity.x < -0.01f)
        {
            renderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        updateAnimator();
    }

    private void updateLocation()
    {
        if (!leaving)
        {
            ds.target = pointsList[i];
        }
    }


    // IEnumerator updateLocation()
    // {
    //     updatingLocation = true;
    //     if (!leaving)
    //     {
    //         yield return new WaitForSeconds(timeBetweenPoints);
    //         i = (i + 1) % pointsList.Count;
    //         ds.target = pointsList[i];
    //         updatingLocation = false;
    //     }
    // }

    public void Leave()
    {
        leaving = true;
        ds.target = exit;
        foreach (var comp in GetComponents<Collider2D>())
        {
            comp.enabled = false;
        }
        
        //Invoke(nameof(kill), 15 );
    }

    private void kill()
    {
        Destroy(this);
    }
}
