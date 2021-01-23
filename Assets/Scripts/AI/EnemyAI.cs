using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEditor.Build;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private float movementThreshold = 0.2f;
    private int i;
    
    private Vector2 curPos;
    private float angleAnim;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        ds = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        ds.target = pointsList[i];
        curPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        updateAnimator();
        
        
        if (Vector2.Distance(transform.position, pointsList[i].position) < thresh)
        {
            i = (i + 1) % pointsList.Count;
            Invoke(nameof(updateLocation), timeBetweenPoints);

            if (i < spawnList.Count)
            {
                GameObject spawn =  Instantiate(Resources.Load(spawnList[i])) as GameObject;
                if (!(spawn is null))
                {
                    float angle = Random.value * 360;
                    float radius = Random.value * dropOffset;
                    Vector2 spawnLocation = new Vector2(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle));
                    spawnLocation += (Vector2) transform.position;
                    spawn.transform.position = spawnLocation;
                }
                else
                {
                    Debug.Log("here");
                }
            }
        }
    }

    private void updateAnimator()
    {
        Vector2 pos = transform.position;
        Vector2 movementDirection = pos - curPos;
        curPos = pos;
        if (Vector2.Distance(pos, movementDirection) < movementThreshold)
        {
            anim.SetBool("moving", false);
        }
        else
        {
            anim.SetBool("moving", true);
        }
        
        angleAnim = Vector2.Angle(Vector2.up, movementDirection);
        if (movementDirection.x > 0)
        {
            anim.SetBool("movingRight", true);
        }
        if (movementDirection.x < 0)
        {
            anim.SetBool("movingRight", false);
        }
        anim.SetFloat("angle", angleAnim);
    }
    private void updateLocation()
    {
        ds.target = pointsList[i];
    }
}
