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
    public float lookForFurnitureDistance;
    public float furnitureSearchCooldown;
    public float actionCooldown;
    private float lastActionTime;
    private float lastFurnitureSearch;
    public float thresh = 0.1f;
    public float timeBetweenPoints;
    public List<Transform> pointsList;
    private List<Transform> lastThreePoints;
    public List<string> spawnList;
    public Animator anim;
    private AIPath path;
    private AIDestinationSetter ds;
    private Vector3 lastLocation; // to check if stuck
    private float lastLocationChecked;
    private int sameLocationStrikes;

    private Transform nextDestination;
    private bool leaving;
    private float movementThreshold = 0.001f;
    private int i;

    public Transform exit;
    public float x_limit;
    public float y_limit;
    private float startedAction; // time started
    private bool actionIsDirty;
    public float idleTimeBeforeAction;
    private bool isInAction;
    private Task curFurniture;

    private SpriteRenderer renderer;
    private Task task;


    private bool isMoving;

    private bool updatingLocation;
    // Start is called before the first frame update
    void Start()
    {
        lastActionTime = -1;
        lastThreePoints = new List<Transform>();
        anim = GetComponent<Animator>();
        i = 0;
        nextDestination = pointsList[i];
        ds = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        task = GetComponent<Task>();
        ds.target = nextDestination;
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

        // check if stuck
        if (Time.time - lastLocationChecked >= 0.5f)
        {
            if (lastLocation == transform.position)
            {
                sameLocationStrikes++;
                if (sameLocationStrikes >= 4)
                    pickNextRandomDest();
            }
            else
                sameLocationStrikes = 0;
            
            lastLocationChecked = Time.time;
            lastLocation = transform.position;
        }

        if (isInAction)
        {
            path.canMove = false;
            StartCoroutine(doAction());
            return;
        }

        // updateAnimator();

        
        // do something
        if (!isInAction && !updatingLocation && pointsList[i] && Vector2.Distance(transform.position, ds.target.position) < thresh && !leaving)
        {
            pickNextRandomDest();
            // 30% to make dirty action
            if(Time.time - lastActionTime > actionCooldown && Random.Range(0, 10) < 3)
                startAction(true);
        }

        // var nn = AstarPath.active.GetNearest(transform.position, NNConstraint.Default);
        // var closestPointOnGraph = nn.clampedPosition;
        // transform.position = (Vector2) closestPointOnGraph;
    }

    private void pickNextRandomDest()
    {
        if (Time.time - lastFurnitureSearch > furnitureSearchCooldown)
        {
            lastFurnitureSearch = Time.time;
            // 50% to go to close furniture
            if (Random.Range(0, 10) < 5)
            {
                if (goClosestFurniture())
                {
                    Invoke(nameof(updateLocation), timeBetweenPoints);
                    return;
                }
            }
        }
        int nextIndex = Random.Range(0, pointsList.Count);
        int tries = 0;
        while (tries++ < 20 && lastThreePoints.Contains(pointsList[nextIndex]))
        {
            nextIndex = Random.Range(0, pointsList.Count);
        }
        
        i = nextIndex;
        nextDestination = pointsList[nextIndex];
        lastThreePoints.Add(nextDestination);
        if (lastThreePoints.Count > 3)
            lastThreePoints.RemoveAt(3);
        
        Invoke(nameof(updateLocation), timeBetweenPoints);
    }

    IEnumerator doAction()
    {
        if (startedAction >= 0)
        {
            startedAction = -1;
            anim.SetInteger("Action", (int) ActionType.Idle);
            yield return new WaitForSeconds(idleTimeBeforeAction);
            
            if (actionIsDirty)
                yield return dirtyAction();
            else
                yield return breakAction();
            
            anim.SetInteger("Action", (int) ActionType.Idle);
            isMoving = false;
            path.canMove = true;
            isInAction = false;
        }
    }

    private IEnumerator breakAction()
    {
        anim.SetInteger("Action", (int) ActionType.Break);
        AudioManager.i.PlaySound(AudioFileGetter.i.Break);
        yield return new WaitForSeconds(18f / 24f);
        curFurniture.breakFurniture();
    }

    private IEnumerator dirtyAction()
    {
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
    }

    private void startAction(bool actionDirty)
    {
        isInAction = true;
        startedAction = Time.time;
        actionIsDirty = actionDirty;
        lastActionTime = Time.time;
    }


    private void updateAnimator()
    {
        Vector3 velocity = path.velocity;
        if (isMoving && (velocity == Vector3.zero || path.velocity.magnitude < movementThreshold))
        {
            isMoving = false;
            anim.SetInteger("Action", (int) ActionType.Idle);
        }
        else if (!isMoving && (velocity != Vector3.zero && path.velocity.magnitude > movementThreshold))
        {
            isMoving = true;
            anim.SetInteger("Action", (int) ActionType.Move);
        }

        if (velocity.x > 0.01f)
            renderer.flipX = true;

        if (velocity.x < -0.01f)
            renderer.flipX = false;
    }

    private void FixedUpdate()
    {
        updateAnimator();
    }

    private void updateLocation()
    {
        if (!leaving)
        {
            ds.target = nextDestination;
        }
    }

    private bool goClosestFurniture()
    {
        List<Task> furnitures = GameManager.Instance.getFurnitures();
        List<Task> closeBreakableFurnitures = new List<Task>();
        foreach(Task furniture in furnitures)
            if(furniture.canBreak() && Vector3.Distance(gameObject.transform.position, furniture.gameObject.transform.position) < lookForFurnitureDistance)
                closeBreakableFurnitures.Add(furniture);
        
        if (closeBreakableFurnitures.Count == 0)
            return false;
        
        // pick random furniture to go to
        int destIndex = Random.Range(0, closeBreakableFurnitures.Count);
        nextDestination = closeBreakableFurnitures[destIndex].transform;
        return true;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Task"))
        {
            Task otherTask = GameManager.Instance.getTask(other.gameObject);
            if (otherTask.type != TaskType.Furniture || wallBetween(other.gameObject))
                return;

            // 50% chance to break
            if (Time.time - lastActionTime > actionCooldown && otherTask.canBreak() && Random.Range(0, 10) < 5)
            {
                curFurniture = otherTask;
                startAction(false);
            }
        }
    }
    
    private bool wallBetween(GameObject go)
    {
        float leftBottomX = Mathf.Min(transform.position.x, go.transform.position.x);
        float leftBottomY = Mathf.Min(transform.position.y, go.transform.position.y);
        float rightTopX = Mathf.Max(transform.position.x, go.transform.position.x);
        float rightTopY = Mathf.Max(transform.position.y, go.transform.position.y);
        
        Vector2 center = new Vector2(leftBottomX + (rightTopX - leftBottomX)/2, leftBottomY + (rightTopY - leftBottomY)/2);
        Bounds rayLike = new Bounds(center, new Vector2(rightTopX - leftBottomX, rightTopY - leftBottomY));
        return !GameManager.Instance.isInWall(rayLike);
    }
}
