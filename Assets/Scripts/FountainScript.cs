using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainScript : MonoBehaviour
{
    public float secondsToMax;
    private SpriteRenderer renderer;
    private GameObject puddle;
    private SpriteRenderer puddleRenderer;

    private Vector2 initialPuddleScale;

    private Vector2 maxScale = new Vector2(0.2f, 0.2f);
    private Vector2 scalePerSecond;

    private Task puddleTask;
    private float initialPuddleTime;
    // Start is called before the first frame update
    void Start()
    {
        secondsToMax = 45;
        renderer = GetComponent<SpriteRenderer>();
        
        initialPuddleScale = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!puddle)
        {
            puddle = Instantiate(Resources.Load("Puddle")) as GameObject;
            // position
            puddleRenderer = puddle.GetComponent<SpriteRenderer>();
            puddleTask = puddle.GetComponent<Task>();
            initialPuddleTime = puddleTask.duration;
            puddle.transform.position = transform.position + new Vector3(0, -renderer.bounds.extents.y - puddleRenderer.bounds.extents.y);
            Vector2 extents = Vector2.zero;
            while (MyGameManager.Instance.isInWall(puddleRenderer.bounds))
            {
                extents += (Vector2)puddleRenderer.bounds.extents;
                setNewPuddlePosition(extents);
            }

            // scale per second
            if(initialPuddleScale == Vector2.zero)
                initialPuddleScale = puddle.transform.localScale;
            scalePerSecond = (maxScale - initialPuddleScale) / secondsToMax;
        }
        
        if (puddle && !MyGameManager.Instance.getPlayerScript().isFixing && puddle.transform.localScale.x < maxScale.x)
        {
            Vector2 addedExtents = puddleRenderer.bounds.extents;
            puddle.transform.localScale += (Vector3)(scalePerSecond * Time.deltaTime);
            puddleTask.duration = initialPuddleTime + (puddle.transform.localScale.x - initialPuddleScale.x) / initialPuddleScale.x;
            
            Vector2 extents = addedExtents;
            while (MyGameManager.Instance.isInWall(puddleRenderer.bounds))
            {
                setNewPuddlePosition(extents);
                extents += new Vector2(0.001f, 0.001f);
            }
        }
    }

    private void setNewPuddlePosition(Vector2 addedExtents)
    {
        Bounds puddleBounds = puddleRenderer.bounds;
        if (MyGameManager.Instance.isInWall(puddleBounds))
        {
            if(addedExtents != (Vector2)puddleBounds.extents)
                addedExtents = (Vector2) puddleBounds.extents - addedExtents;
            // move right
            Vector3 offsetRight = new Vector3(addedExtents.x, 0, 0);
            puddleBounds.center = puddle.transform.position + offsetRight;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
            }
            // move left
            Vector3 offsetLeft = new Vector3(-addedExtents.x, 0, 0);
            puddleBounds.center = puddle.transform.position + offsetLeft;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
                return;
            }
            // move up
            Vector3 offsetUp = new Vector3(0, addedExtents.y, 0);
            puddleBounds.center = puddle.transform.position + offsetUp;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
                return;
            }
            // move down
            Vector3 offsetDown = new Vector3(0, -addedExtents.y, 0);
            puddleBounds.center = puddle.transform.position + offsetDown;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
                return;
            }
            
            // move up right
            puddleBounds.center = puddle.transform.position + offsetUp + offsetRight;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
                return;
            }
            
            // move up left
            puddleBounds.center = puddle.transform.position + offsetUp + offsetLeft;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
                return;
            }
            
            // move down right
            puddleBounds.center = puddle.transform.position + offsetDown + offsetRight;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
                return;
            }
            
            // move down left
            puddleBounds.center = puddle.transform.position + offsetDown + offsetLeft;
            if (!MyGameManager.Instance.isInWall(puddleBounds))
            {
                puddle.transform.position = puddleBounds.center;
                return;
            }
        }
    }
}
