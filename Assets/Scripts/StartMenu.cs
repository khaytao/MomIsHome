using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartPlaying()
    {
        transform.localScale = 1.1f * transform.localScale;
    }
}
