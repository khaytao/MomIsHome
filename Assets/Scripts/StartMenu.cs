using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    public void StartPlaying()
    {
        SceneManager.LoadScene(1);
        GameManager.Instance.LoadLevelPrefabs(1);
    }
}
