using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Color color;
    [HideInInspector] public Collider2D itemCollider;
    void Start()
    {
        itemCollider = GetComponent<Collider2D>();
        GameManager.Instance.addItem(this);
        color = GetComponent<SpriteRenderer>().color;
    }
}
