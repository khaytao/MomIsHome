using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireScript : MonoBehaviour
{
    public float spreadTimeInterval;
    private float lastCheck;
    private BoxCollider2D boxCollider;
    private Bounds boxBounds;
    private CircleCollider2D circleCollider;
    private SpriteRenderer fireRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        fireRenderer = GetComponent<SpriteRenderer>();
        boxBounds = boxCollider.bounds;
        // boxBounds = GetComponent<SpriteRenderer>().bounds;
        MyGameManager.Instance.addFire(gameObject, boxBounds);
        lastCheck = Time.time;
        PlayerScript playerScript = MyGameManager.Instance.getPlayerScript();
        if (playerScript != null && circleCollider.bounds.Contains(playerScript.transform.position))
        {
            playerScript.addToTask(GetComponent<Task>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheck >= spreadTimeInterval)
        {
            lastCheck = Time.time;
            // 50% chance to generate fire
            if (Random.Range(0, 10) >= 5)
            {
                Vector2 dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
                Bounds temp = boxBounds;
                Vector2 center = gameObject.transform.position;
                // center.x += dir.x * spreadDistance.x;
                // center.y += dir.y * spreadDistance.y;
                center.x += dir.x * boxBounds.size.x;
                center.y += dir.y * boxBounds.size.y;
                temp.center = center;
                if (MyGameManager.Instance.isInFire(temp) || MyGameManager.Instance.isInWall(temp))
                    return;
                
                // center.x -= dir.x * boxBounds.extents.x * 0.5f;
                // center.y -= dir.y * boxBounds.extents.y * 0.5f;
                GameObject newFire = Instantiate(Resources.Load("Fire"), new Vector3(center.x, center.y), Quaternion.identity) as GameObject;

                // burn furniture if on them
                // Task furniture = GameManager.Instance.inFurniture(new Bounds(center, boxBounds.extents));
                // if (furniture != null)
                // {
                //     furniture.burnFurniture();
                // }
            }
        }
    }

    private void OnDestroy()
    {
        // weird exception
        if (!gameObject)
            return;
        if (!MyGameManager.Instance)
            return;
        MyGameManager.Instance.removeFire(gameObject);
    }
}
