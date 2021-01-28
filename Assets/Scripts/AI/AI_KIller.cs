using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_KIller : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Task"))
        {
            Task script = other.GetComponent<Task>();
            if (script.type == TaskType.NPC)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
