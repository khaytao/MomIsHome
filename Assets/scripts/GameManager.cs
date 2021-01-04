using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private float timeStarted;
    private bool gameOver;
    // how much time has elapsed since the start of game (in case there is an opening scene its not equal to Time.time)
    public float elapsedTime { get
        {
            if (timeStarted == -1)
                timeStarted = Time.time;
            return Time.time - timeStarted;
        }
    }
    private Dictionary<GameObject, Item> goToItem;
    private Dictionary<GameObject, Task> goToTask;
    
    public GameManager()
    {
        goToItem = new Dictionary<GameObject, Item>();
        goToTask = new Dictionary<GameObject, Task>();
        timeStarted = -1;
        gameOver = false;
    }

    public void addItem(Item item)
    {
        goToItem.Add(item.gameObject, item);
    }

    public void addTask(Task task)
    {
        goToTask.Add(task.gameObject, task);
    }

    public Item getItem(GameObject go)
    {
        return goToItem[go];
    }

    public Task getTask(GameObject go)
    {
        return goToTask[go];
    }

    public void timeOver()
    {
        Debug.Log("Game over!");
        gameOver = true;
    }

    public bool isGameOver()
    {
        return gameOver || (goToTask.Count <= 0);
    }
}
