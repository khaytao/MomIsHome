using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D boxCollider in boxColliders)
        {
            GameManager.Instance.addWallBounds(new Bounds(boxCollider.bounds.center, new Vector3(boxCollider.size.x, boxCollider.size.y, 0)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
