using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public List<Transform> pointsList;
    public float thresh = 0.1f;
    private AIPath path;
    private AIDestinationSetter ds;

    private int i;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        ds = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        ds.target = pointsList[i];
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, pointsList[i].position) < thresh)
        {
            Debug.Log("here");
            i = (i + 1) % pointsList.Count;
            ds.target = pointsList[i];
            
        }
    }
}
