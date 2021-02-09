using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetWorldspaceCam : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Canvas>().worldCamera = Camera.current;
    }
}
