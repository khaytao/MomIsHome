using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public float spreadTimeInterval;
    private float lastCheck;
    private BoxCollider2D boxCollider;
    private Bounds boxBounds;
    private CircleCollider2D circleCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        boxBounds = boxCollider.bounds;
        // boxBounds = GetComponent<SpriteRenderer>().bounds;
        GameManager.Instance.addFire(boxBounds);
        lastCheck = Time.time;
        PlayerScript playerScript = GameManager.Instance.getPlayerScript();
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
            // 60% chance to generate fire
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
                if (GameManager.Instance.isInFireOrWall(temp))
                    return;
                
                center.x -= dir.x * boxBounds.extents.x * 0.5f;
                center.y -= dir.y * boxBounds.extents.y * 0.5f;
                GameObject newFire = Instantiate(Resources.Load("Fire")) as GameObject;
                newFire.transform.position = center;
            }
        }
    }
}
