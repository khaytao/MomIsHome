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
        GameManager.Instance.addFire(gameObject, boxBounds);
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
            // 50% chance to generate fire
            if (Random.Range(0, 10) >= 5)
            {
                Vector2 dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
                Bounds temp = boxBounds;
                Vector2 center = gameObject.transform.position;
                center.x += dir.x * boxBounds.size.x;
                center.y += dir.y * boxBounds.size.y;
                temp.center = center;
                if (GameManager.Instance.isInFire(temp) || GameManager.Instance.isInWall(temp))
                    return;

                GameObject newFire = Instantiate(Resources.Load("Fire"), new Vector3(center.x, center.y), Quaternion.identity) as GameObject;
            }
        }
    }

    private void OnDestroy()
    {
        // weird exception
        if (!gameObject)
            return;
        if (!GameManager.Instance)
            return;
        GameManager.Instance.removeFire(gameObject);
    }
}
