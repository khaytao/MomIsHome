using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPointScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyGameManager.Instance.addAIPoint(gameObject);
    }
}
