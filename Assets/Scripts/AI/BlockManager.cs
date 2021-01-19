using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform block in transform)
        {
            block.gameObject.SetActive(false);
            //Collider2D col = block.GetComponent<Collider2D>();
            //col.enabled = false;
        }
    }
}
