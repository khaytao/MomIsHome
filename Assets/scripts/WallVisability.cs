using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallVisability : MonoBehaviour
{

    private SpriteRenderer _sr;
    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D boxCollider in boxColliders)
        {
            GameManager.Instance.addWallBounds(boxCollider.bounds);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _sr.color = new Color(1f,1f,1f,.5f);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _sr.color = new Color(1f,1f,1f,1f);
        }
    }
}
